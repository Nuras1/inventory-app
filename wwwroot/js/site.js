const html = document.documentElement;
const themeSwitch = document.getElementById('btnSwitch');

if (themeSwitch) {

    const savedTheme = localStorage.getItem('theme');

    if (savedTheme) {

        html.setAttribute('data-bs-theme', savedTheme);

        if (savedTheme === 'dark') {

            themeSwitch.checked = true;
        }
    }

    themeSwitch.addEventListener('change', function () {

        if (this.checked) {

            html.setAttribute('data-bs-theme', 'dark');

            localStorage.setItem('theme', 'dark');

        } else {

            html.setAttribute('data-bs-theme', 'light');

            localStorage.setItem('theme', 'light');
        }
    });
}

const searchInput = document.getElementById("searchInput");
const clearButton = document.getElementById("clearSearch");

if (searchInput && clearButton) {

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

setTimeout(function () {

    let alert = document.getElementById("autoCloseAlert");

    if (alert) {

        let bsAlert = new bootstrap.Alert(alert);

        bsAlert.close();
    }

}, 3000);