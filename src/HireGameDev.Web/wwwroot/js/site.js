// HireGameDev — colour theme switcher (light / dark / system). See ADR-015.
//
// The preference is stored in localStorage under "hgd-theme" ("light" | "dark";
// absent = follow the system). The resolved mode is written to <html data-bs-theme>,
// which Bootstrap 5.3 and css/site.css both key off. _Layout.cshtml applies the
// stored value inline in <head> before paint, so this file only has to keep the
// UI in sync and react to changes.
(function () {
    "use strict";

    var STORAGE_KEY = "hgd-theme";
    var systemDark = window.matchMedia("(prefers-color-scheme: dark)");

    function readPreference() {
        try {
            var value = window.localStorage.getItem(STORAGE_KEY);
            return value === "light" || value === "dark" ? value : "system";
        } catch (e) {
            return "system";
        }
    }

    function writePreference(preference) {
        try {
            if (preference === "system") {
                window.localStorage.removeItem(STORAGE_KEY);
            } else {
                window.localStorage.setItem(STORAGE_KEY, preference);
            }
        } catch (e) {
            // Storage unavailable (private mode, blocked): the choice simply lasts for this page.
        }
    }

    function resolve(preference) {
        if (preference === "system") {
            return systemDark.matches ? "dark" : "light";
        }
        return preference;
    }

    function apply(preference) {
        document.documentElement.setAttribute("data-bs-theme", resolve(preference));

        // Browser chrome colour on mobile follows the page background token.
        var themeColor = document.querySelector('meta[name="theme-color"]');
        if (themeColor) {
            var background = getComputedStyle(document.documentElement).getPropertyValue("--hgd-bg").trim();
            if (background) {
                themeColor.setAttribute("content", background);
            }
        }

        document.querySelectorAll("[data-hgd-theme-value]").forEach(function (option) {
            var selected = option.getAttribute("data-hgd-theme-value") === preference;
            option.classList.toggle("active", selected);
            option.setAttribute("aria-checked", selected ? "true" : "false");
        });

        document.querySelectorAll("[data-hgd-theme-icon]").forEach(function (icon) {
            icon.classList.toggle("d-none", icon.getAttribute("data-hgd-theme-icon") !== preference);
        });
    }

    document.addEventListener("DOMContentLoaded", function () {
        apply(readPreference());

        document.querySelectorAll("[data-hgd-theme-value]").forEach(function (option) {
            option.addEventListener("click", function () {
                var preference = option.getAttribute("data-hgd-theme-value");
                writePreference(preference);
                apply(preference);
            });
        });
    });

    // Follow the OS while "system" is selected.
    var onSystemChange = function () {
        if (readPreference() === "system") {
            apply("system");
        }
    };
    if (typeof systemDark.addEventListener === "function") {
        systemDark.addEventListener("change", onSystemChange);
    } else if (typeof systemDark.addListener === "function") {
        systemDark.addListener(onSystemChange);
    }

    // Keep other open tabs in step.
    window.addEventListener("storage", function (event) {
        if (event.key === STORAGE_KEY || event.key === null) {
            apply(readPreference());
        }
    });
})();
