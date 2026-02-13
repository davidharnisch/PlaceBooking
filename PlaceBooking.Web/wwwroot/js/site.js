// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');
    [...tooltipTriggerList].forEach((tooltipTriggerEl) => {
        new bootstrap.Tooltip(tooltipTriggerEl);
    });
})();

// date picker submit (uses requestSubmit when available)
document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.room-date-input').forEach(function (input) {
        input.addEventListener('change', function (e) {
            var form = e.target.form;
            if (!form) return;
            if (typeof form.requestSubmit === 'function') {
                form.requestSubmit();
            } else {
                form.submit();
            }
        });
    });
});

// Password visibility toggle
(() => {
    function setupPasswordToggles(root = document) {
        root.querySelectorAll('.btn-toggle-password').forEach(function (btn) {
            if (btn.dataset.toggleBound) return; // avoid double-binding

            var targetId = btn.getAttribute('data-target');
            if (!targetId) return;

            var input = document.getElementById(targetId);
            var icon = btn.querySelector('i');
            if (!input || !icon) return;

            btn.addEventListener('click', function () {
                var isHidden = input.type === 'password';
                input.type = isHidden ? 'text' : 'password';
                icon.classList.toggle('bi-eye', !isHidden);
                icon.classList.toggle('bi-eye-slash', isHidden);
            });

            btn.dataset.toggleBound = '1';
        });
    }

    // expose for manual re-initialization (e.g. after DOM injection)
    window.setupPasswordToggles = setupPasswordToggles;

    document.addEventListener('DOMContentLoaded', function () {
        setupPasswordToggles(document);
    });
})();
