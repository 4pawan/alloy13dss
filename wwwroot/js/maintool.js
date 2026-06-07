(function () {
    function copyCurrentValue(form) {
        var activeInput = form.querySelector('[data-f-element-name] input:not([type=hidden]), [data-f-element-name] textarea, [data-f-element-name] select');
        var valueInput = form.querySelector('[data-dummy-current-value]');

        if (activeInput && valueInput) {
            valueInput.value = activeInput.value || '';
        }
    }

    function ensureRecaptcha(form, event) {
        var siteKey = form.getAttribute('data-dummy-recaptcha-site-key');
        var action = form.getAttribute('data-dummy-recaptcha-action') || 'dummy_form_step';
        var tokenInput = form.querySelector('[data-dummy-recaptcha-token]');

        if (!siteKey || !tokenInput || tokenInput.value || !window.grecaptcha) {
            return false;
        }

        event.preventDefault();
        window.grecaptcha.ready(function () {
            window.grecaptcha.execute(siteKey, { action: action }).then(function (token) {
                tokenInput.value = token;
                form.submit();
            });
        });

        return true;
    }

    function updateActiveQuestionTitle(form) {
        var title = form.getAttribute('data-dummy-active-question-title') || '';
        var targets = document.querySelectorAll('[data-dummy-active-question-title-target]');

        for (var index = 0; index < targets.length; index++) {
            targets[index].textContent = title;
            targets[index].hidden = !title;
        }
    }

    function wireForm(form) {
        if (form.getAttribute('data-dummy-form-ready') === 'true') {
            return;
        }

        form.setAttribute('data-dummy-form-ready', 'true');
        updateActiveQuestionTitle(form);

        form.addEventListener('submit', function (event) {
            copyCurrentValue(form);
            ensureRecaptcha(form, event);
        });
    }

    function init() {
        var forms = document.querySelectorAll('[data-dummy-form]');
        for (var index = 0; index < forms.length; index++) {
            wireForm(forms[index]);
        }
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init);
    } else {
        init();
    }
})();
