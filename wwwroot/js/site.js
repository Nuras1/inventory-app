const html = document.documentElement;
const themeSwitch = document.getElementById("btnSwitch");

if (themeSwitch) {

    const savedTheme = localStorage.getItem("theme");

    if (savedTheme) {

        html.setAttribute("data-bs-theme", savedTheme);

        if (savedTheme === "dark") {
            themeSwitch.checked = true;
        }

    }

    themeSwitch.addEventListener("change", function () {

        if (this.checked) {

            html.setAttribute("data-bs-theme", "dark");

            localStorage.setItem("theme", "dark");

        }
        else {

            html.setAttribute("data-bs-theme", "light");

            localStorage.setItem("theme", "light");

        }

    });

}

setTimeout(function () {

    const alertElement =
        document.getElementById("autoCloseAlert");

    if (alertElement) {

        const bsAlert =
            new bootstrap.Alert(alertElement);

        bsAlert.close();

    }

}, 3000);

document.addEventListener("DOMContentLoaded", () => {

    initClickableRows();

    initUserToolbar();

    initInventoryToolbar();

    initTagSuggestions();

    initSearchClear();

});

function initClickableRows() {

    document.querySelectorAll(".clickable-row")
        .forEach(row => {

            row.addEventListener("click", (e) => {

                if (e.target.closest("a")) {
                    return;
                }

                const href = row.dataset.href;

                if (href) {
                    window.location.href = href;
                }

            });

        });

}

function initUserToolbar() {

    const selectors =
        document.querySelectorAll(".user-selector");

    const toolbar =
        document.getElementById("bulkToolbar");

    if (!selectors.length || !toolbar) {
        return;
    }

    selectors.forEach(selector => {

        selector.addEventListener("change", function () {

            const userId = this.value;

            toolbar.classList.remove("d-none");

            document.getElementById("blockUserId").value =
                userId;

            document.getElementById("unblockUserId").value =
                userId;

            document.getElementById("makeAdminUserId").value =
                userId;

            document.getElementById("removeAdminUserId").value =
                userId;

            document.getElementById("deleteUserId").value =
                userId;

        });

    });

}

function initInventoryToolbar() {

    const selectors =
        document.querySelectorAll(".inventory-selector");

    const toolbar =
        document.getElementById("bulkToolbar");

    if (!selectors.length || !toolbar) {
        return;
    }

    const openBtn =
        document.getElementById("openInventoryBtn");

    const editBtn =
        document.getElementById("editInventoryBtn");

    const deleteBtn =
        document.getElementById("deleteInventoryBtn");

    selectors.forEach(selector => {

        selector.addEventListener("change", function () {

            const inventoryId = this.value;

            toolbar.classList.remove("d-none");

            openBtn.href =
                `/Inventory/Details/${inventoryId}`;

            editBtn.href =
                `/Inventory/Edit/${inventoryId}`;

            deleteBtn.href =
                `/Inventory/Delete/${inventoryId}`;

        });

    });

}

function initTagSuggestions() {

    const tagsInput =
        document.getElementById("tagsInput");

    const suggestionsBox =
        document.getElementById("tagsSuggestions");

    if (!tagsInput || !suggestionsBox) {
        return;
    }

    tagsInput.addEventListener("input", async () => {

        const parts =
            tagsInput.value.split(",");

        const current =
            parts[parts.length - 1].trim();

        if (current.length < 1) {

            suggestionsBox.classList.add("d-none");

            suggestionsBox.innerHTML = "";

            return;

        }

        const response =
            await fetch(`/Inventory/SearchTags?term=${encodeURIComponent(current)}`);

        const tags =
            await response.json();

        suggestionsBox.innerHTML = "";

        if (tags.length === 0) {

            suggestionsBox.classList.add("d-none");

            return;

        }

        tags.forEach(tag => {

            const item =
                document.createElement("div");

            item.className =
                "tag-suggestion-item";

            item.textContent = tag;

            item.addEventListener("click", () => {

                parts[parts.length - 1] =
                    ` ${tag}`;

                tagsInput.value =
                    parts.join(",").replace(/^ /, "");

                suggestionsBox.classList.add("d-none");

            });

            suggestionsBox.appendChild(item);

        });

        suggestionsBox.classList.remove("d-none");

    });

    document.addEventListener("click", (e) => {

        if (!suggestionsBox.contains(e.target) &&
            e.target !== tagsInput) {

            suggestionsBox.classList.add("d-none");

        }

    });

}

function initSearchClear() {

    const searchInput =
        document.getElementById("searchInput");

    const clearButton =
        document.getElementById("clearSearch");

    if (!searchInput || !clearButton) {
        return;
    }

    function toggleClear() {

        clearButton.style.display =
            searchInput.value.length > 0
                ? "block"
                : "none";

    }

    toggleClear();

    searchInput.addEventListener("input", toggleClear);

    clearButton.addEventListener("click", function () {

        searchInput.value = "";

        toggleClear();

        searchInput.focus();

    });

}