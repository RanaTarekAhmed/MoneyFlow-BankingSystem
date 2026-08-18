/**
 * MoneyFlow — Frontend UI Interactions (Vanilla JavaScript)
 */

document.addEventListener('DOMContentLoaded', () => {
    // 1. Password Visibility Toggle
    const toggleButtons = document.querySelectorAll('.mf-password-toggle-btn');
    toggleButtons.forEach(btn => {
        btn.addEventListener('click', (e) => {
            e.preventDefault();
            const targetId = btn.getAttribute('data-target');
            const input = document.getElementById(targetId);
            if (!input) return;

            const isPassword = input.getAttribute('type') === 'password';
            input.setAttribute('type', isPassword ? 'text' : 'password');

            const eyeOpen = btn.querySelector('.mf-eye-open');
            const eyeClosed = btn.querySelector('.mf-eye-closed');

            if (eyeOpen && eyeClosed) {
                if (isPassword) {
                    eyeOpen.classList.add('d-none');
                    eyeClosed.classList.remove('d-none');
                } else {
                    eyeOpen.classList.remove('d-none');
                    eyeClosed.classList.add('d-none');
                }
            }
        });
    });

    // 2. Password Strength Meter & Live Requirements (Register Page)
    const regPassword = document.getElementById('reg-password');
    if (regPassword) {
        const bar1 = document.getElementById('str-bar-1');
        const bar2 = document.getElementById('str-bar-2');
        const bar3 = document.getElementById('str-bar-3');
        const strLabel = document.getElementById('str-label');

        const reqLength = document.getElementById('req-length');
        const reqUppercase = document.getElementById('req-uppercase');
        const reqNumber = document.getElementById('req-number');
        const reqSpecial = document.getElementById('req-special');

        const updateReqItem = (el, isMet) => {
            if (!el) return;
            if (isMet) {
                el.classList.add('met');
                const icon = el.querySelector('svg');
                if (icon) {
                    icon.innerHTML = '<path d="M9 16.17L4.83 12l-1.42 1.41L9 19 21 7l-1.41-1.41z"/>';
                }
            } else {
                el.classList.remove('met');
                const icon = el.querySelector('svg');
                if (icon) {
                    icon.innerHTML = '<circle cx="12" cy="12" r="6"/>';
                }
            }
        };

        regPassword.addEventListener('input', () => {
            const val = regPassword.value;

            const hasMinLen = val.length >= 8;
            const hasUpper = /[A-Z]/.test(val);
            const hasNumber = /[0-9]/.test(val);
            const hasSpecial = /[^A-Za-z0-9]/.test(val);

            updateReqItem(reqLength, hasMinLen);
            updateReqItem(reqUppercase, hasUpper);
            updateReqItem(reqNumber, hasNumber);
            updateReqItem(reqSpecial, hasSpecial);

            const score = [hasMinLen, hasUpper, hasNumber, hasSpecial].filter(Boolean).length;

            // Reset bars
            [bar1, bar2, bar3].forEach(bar => {
                if (bar) bar.className = 'mf-strength-bar';
            });

            if (val.length === 0) {
                if (strLabel) strLabel.textContent = 'Password strength: Weak';
                return;
            }

            if (score <= 2) {
                if (bar1) bar1.classList.add('active-weak');
                if (strLabel) strLabel.textContent = 'Password strength: Weak';
            } else if (score === 3) {
                if (bar1) bar1.classList.add('active-medium');
                if (bar2) bar2.classList.add('active-medium');
                if (strLabel) strLabel.textContent = 'Password strength: Medium';
            } else if (score === 4) {
                if (bar1) bar1.classList.add('active-strong');
                if (bar2) bar2.classList.add('active-strong');
                if (bar3) bar3.classList.add('active-strong');
                if (strLabel) strLabel.textContent = 'Password strength: Strong';
            }
        });
    }

    // 3. Smooth scrolling for hash links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href');
            if (targetId === '#' || targetId === '#forgot-password') return;
            const target = document.querySelector(targetId);
            if (target) {
                e.preventDefault();
                target.scrollIntoView({
                    behavior: 'smooth',
                    block: 'start'
                });
            }
        });
    });
});
