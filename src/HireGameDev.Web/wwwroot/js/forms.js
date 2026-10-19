// HireGameDev — small form behaviours, wired by data attributes so any page can reuse them
// without page-specific script (ADR-012: vanilla JS only where needed).
//
//   Repeater  — add/remove rows bound to a List<T> on the page model.
//       <div data-hgd-repeater data-hgd-repeater-prefix="Input.Items" data-hgd-repeater-max="6">
//         <div data-hgd-repeater-rows> …rows with [data-hgd-repeater-row]… </div>
//         <template data-hgd-repeater-template> …one row whose names use Input.Items[__index__]… </template>
//         <p data-hgd-repeater-empty>…</p>  <button data-hgd-repeater-add>…</button>
//       </div>
//     Rows are re-numbered after a removal so MVC binds a contiguous list.
//
//   Filter    — <input data-hgd-filter data-hgd-filter-target="#picker"> hides [data-hgd-filter-item]
//               elements that do not contain the text, [data-hgd-filter-group]s left empty, and
//               shows [data-hgd-filter-empty] when nothing matches.
//
//   Counters  — [data-hgd-checked-count data-hgd-checked-count-for="Input.SkillIds" data-hgd-checked-min="3"]
//               shows how many boxes of that name are checked; [data-hgd-charcount-for="elementId"]
//               mirrors a textarea's length.
//   City      — <select data-hgd-city-state="#state-label"> whose selected option has data-state;
//               updates the text of the matching [data-hgd-city-state-output] element.
(function () {
    "use strict";

    function escapeRegExp(text) {
        return text.replace(/[.*+?^${}()|[\]\\]/g, "\\$&");
    }

    function reparseValidation(form) {
        var $ = window.jQuery;
        if (form && $ && $.validator && $.validator.unobtrusive) {
            $(form).removeData("validator").removeData("unobtrusiveValidation");
            $.validator.unobtrusive.parse(form);
        }
    }

    // ---- Repeater -----------------------------------------------------------------
    function initRepeater(container) {
        var prefix = container.getAttribute("data-hgd-repeater-prefix");
        var max = parseInt(container.getAttribute("data-hgd-repeater-max"), 10) || Infinity;
        var rows = container.querySelector("[data-hgd-repeater-rows]");
        var template = container.querySelector("template[data-hgd-repeater-template]");
        var addButton = container.querySelector("[data-hgd-repeater-add]");
        var emptyNote = container.querySelector("[data-hgd-repeater-empty]");

        if (!prefix || !rows || !template) {
            return;
        }

        // Names look like Input.Items[3].Field; ids and for/aria targets like Input_Items_3__Field.
        var namePattern = new RegExp(escapeRegExp(prefix) + "\\[(\\d+|__index__)\\]", "g");
        var idPattern = new RegExp(escapeRegExp(prefix.replace(/\./g, "_")) + "_(\\d+|__index__)__", "g");
        var nameAttributes = ["name", "data-valmsg-for"];
        var idAttributes = ["id", "for", "aria-describedby"];

        function renumber() {
            var rowElements = rows.querySelectorAll("[data-hgd-repeater-row]");
            rowElements.forEach(function (row, index) {
                row.querySelectorAll("*").forEach(function (element) {
                    nameAttributes.forEach(function (attribute) {
                        var value = element.getAttribute(attribute);
                        if (value) {
                            element.setAttribute(attribute, value.replace(namePattern, prefix + "[" + index + "]"));
                        }
                    });
                    idAttributes.forEach(function (attribute) {
                        var value = element.getAttribute(attribute);
                        if (value) {
                            element.setAttribute(attribute, value.replace(idPattern, prefix.replace(/\./g, "_") + "_" + index + "__"));
                        }
                    });
                });
            });

            var count = rowElements.length;
            if (addButton) {
                addButton.disabled = count >= max;
            }
            if (emptyNote) {
                emptyNote.classList.toggle("d-none", count > 0);
            }
        }

        function addRow() {
            if (rows.querySelectorAll("[data-hgd-repeater-row]").length >= max) {
                return;
            }
            var fragment = template.content.cloneNode(true);
            rows.appendChild(fragment);
            renumber();
            reparseValidation(container.closest("form"));

            var newRow = rows.querySelector("[data-hgd-repeater-row]:last-child");
            var firstField = newRow && newRow.querySelector("input, select, textarea");
            if (firstField) {
                firstField.focus();
            }
        }

        container.addEventListener("click", function (event) {
            var remove = event.target.closest("[data-hgd-repeater-remove]");
            if (remove && container.contains(remove)) {
                var row = remove.closest("[data-hgd-repeater-row]");
                if (row) {
                    row.remove();
                    renumber();
                }
                return;
            }
            if (addButton && event.target.closest("[data-hgd-repeater-add]") === addButton) {
                addRow();
            }
        });

        renumber();
    }

    // ---- Filter -------------------------------------------------------------------
    function initFilter(input) {
        var target = document.querySelector(input.getAttribute("data-hgd-filter-target"));
        if (!target) {
            return;
        }
        var items = target.querySelectorAll("[data-hgd-filter-item]");
        var groups = target.querySelectorAll("[data-hgd-filter-group]");
        var empty = target.querySelector("[data-hgd-filter-empty]");

        function apply() {
            var query = input.value.trim().toLowerCase();
            var visible = 0;
            items.forEach(function (item) {
                var match = !query || item.textContent.toLowerCase().indexOf(query) !== -1;
                item.classList.toggle("d-none", !match);
                if (match) {
                    visible++;
                }
            });
            groups.forEach(function (group) {
                var any = Array.prototype.some.call(
                    group.querySelectorAll("[data-hgd-filter-item]"),
                    function (item) { return !item.classList.contains("d-none"); });
                group.classList.toggle("d-none", !any);
            });
            if (empty) {
                empty.classList.toggle("d-none", visible > 0);
            }
        }

        input.addEventListener("input", apply);
        input.addEventListener("search", apply);
    }

    // ---- Counters -----------------------------------------------------------------
    function initCheckedCount(output) {
        var name = output.getAttribute("data-hgd-checked-count-for");
        var min = parseInt(output.getAttribute("data-hgd-checked-min"), 10) || 0;
        var form = output.closest("form") || document;

        function update() {
            var count = form.querySelectorAll('input[type="checkbox"][name="' + name + '"]:checked').length;
            output.textContent = String(count);
            output.classList.toggle("text-danger", count < min);
            output.classList.toggle("text-success", count >= min);
        }

        form.addEventListener("change", function (event) {
            if (event.target.name === name) {
                update();
            }
        });
        update();
    }

    function initCityState(select) {
        var output = document.querySelector(select.getAttribute("data-hgd-city-state"));
        if (!output) {
            return;
        }

        function update() {
            var option = select.options[select.selectedIndex];
            var state = option && option.getAttribute("data-state");
            output.textContent = state || "Choose a city";
        }

        select.addEventListener("change", update);
        update();
    }

    function initCharCount(output) {
        var field = document.getElementById(output.getAttribute("data-hgd-charcount-for"));
        if (!field) {
            return;
        }
        var update = function () { output.textContent = String(field.value.length); };
        field.addEventListener("input", update);
        update();
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.querySelectorAll("[data-hgd-repeater]").forEach(initRepeater);
        document.querySelectorAll("[data-hgd-filter]").forEach(initFilter);
        document.querySelectorAll("[data-hgd-checked-count]").forEach(initCheckedCount);
        document.querySelectorAll("[data-hgd-charcount-for]").forEach(initCharCount);
        document.querySelectorAll("[data-hgd-city-state]").forEach(initCityState);
    });
})();
