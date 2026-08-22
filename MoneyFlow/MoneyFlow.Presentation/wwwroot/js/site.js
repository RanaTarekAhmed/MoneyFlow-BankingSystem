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
        const reqLowercase = document.getElementById('req-lowercase');
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
            const hasLower = /[a-z]/.test(val);
            const hasNumber = /[0-9]/.test(val);
            const hasSpecial = /[^A-Za-z0-9]/.test(val);

            updateReqItem(reqLength, hasMinLen);
            updateReqItem(reqUppercase, hasUpper);
            updateReqItem(reqLowercase, hasLower);
            updateReqItem(reqNumber, hasNumber);
            updateReqItem(reqSpecial, hasSpecial);

            const score = [hasMinLen, hasUpper, hasLower, hasNumber, hasSpecial].filter(Boolean).length;

            // Reset bars
            [bar1, bar2, bar3].forEach(bar => {
                if (bar) bar.className = 'mf-strength-bar';
            });

            if (val.length === 0) {
                if (strLabel) strLabel.textContent = 'Password strength: Weak';
                return;
            }

            if (score <= 3) {
                if (bar1) bar1.classList.add('active-weak');
                if (strLabel) strLabel.textContent = 'Password strength: Weak';
            } else if (score === 4) {
                if (bar1) bar1.classList.add('active-medium');
                if (bar2) bar2.classList.add('active-medium');
                if (strLabel) strLabel.textContent = 'Password strength: Medium';
            } else if (score === 5) {
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

    // 4. Initialize Server-Side TempData Toasts
    initServerToasts();
});

/* ==========================================================================
   MoneyFlow — Toast Notification API (MoneyFlowToast)
   ========================================================================== */

(function () {
    const ICONS = {
        success: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><polyline points="20 6 9 17 4 12"/></svg>`,
        error: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><circle cx="12" cy="12" r="10"/><line x1="15" y1="9" x2="9" y2="15"/><line x1="9" y1="9" x2="15" y2="15"/></svg>`,
        warning: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"/><line x1="12" y1="9" x2="12" y2="13"/><line x1="12" y1="17" x2="12.01" y2="17"/></svg>`,
        info: `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><circle cx="12" cy="12" r="10"/><line x1="12" y1="16" x2="12" y2="12"/><line x1="12" y1="8" x2="12.01" y2="8"/></svg>`
    };

    const CLOSE_ICON = `<svg viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round" aria-hidden="true"><line x1="18" y1="6" x2="6" y2="18"/><line x1="6" y1="6" x2="18" y2="18"/></svg>`;

    const DEFAULT_TITLES = {
        success: 'Success',
        error: 'Error',
        warning: 'Warning',
        info: 'Information'
    };

    const DEFAULT_DURATIONS = {
        success: 4500,
        info: 5000,
        warning: 6000,
        error: 7000
    };

    function escapeHTML(str) {
        if (!str) return '';
        const div = document.createElement('div');
        div.textContent = str;
        return div.innerHTML;
    }

    function getOrCreateContainer() {
        let container = document.getElementById('mfToastContainer');
        if (!container) {
            container = document.createElement('div');
            container.id = 'mfToastContainer';
            container.className = 'mf-toast-container';
            container.setAttribute('aria-live', 'polite');
            container.setAttribute('aria-atomic', 'true');
            document.body.appendChild(container);
        }
        return container;
    }

    function showToast(options = {}) {
        const container = getOrCreateContainer();
        const type = (options.type || 'info').toLowerCase();
        const title = options.title !== undefined ? options.title : (DEFAULT_TITLES[type] || 'Notification');
        const message = options.message || '';
        const closable = options.closable !== false;
        
        let duration = options.duration;
        if (duration === undefined) {
            duration = DEFAULT_DURATIONS[type] !== undefined ? DEFAULT_DURATIONS[type] : 5000;
        }

        const toast = document.createElement('div');
        toast.className = `mf-toast mf-toast-${type}`;
        toast.setAttribute('role', (type === 'error' || type === 'warning') ? 'alert' : 'status');

        const iconSvg = ICONS[type] || ICONS.info;

        let toastHtml = `
            <div class="mf-toast-icon-wrap" aria-hidden="true">
                ${iconSvg}
            </div>
            <div class="mf-toast-body">
                ${title ? `<div class="mf-toast-title">${escapeHTML(title)}</div>` : ''}
                <div class="mf-toast-message">${options.isRawHtml ? message : escapeHTML(message)}</div>
            </div>
        `;

        if (closable) {
            toastHtml += `
                <button type="button" class="mf-toast-close" aria-label="Close notification">
                    ${CLOSE_ICON}
                </button>
            `;
        }

        if (duration > 0) {
            toastHtml += `
                <div class="mf-toast-progress" aria-hidden="true">
                    <div class="mf-toast-progress-bar" style="width: 100%;"></div>
                </div>
            `;
        }

        toast.innerHTML = toastHtml;
        container.appendChild(toast);

        const closeBtn = toast.querySelector('.mf-toast-close');
        if (closeBtn) {
            closeBtn.addEventListener('click', (e) => {
                e.stopPropagation();
                dismissToast(toast);
            });
        }

        // Handle auto-dismiss and progress bar with pause on hover
        if (duration > 0) {
            const progressBar = toast.querySelector('.mf-toast-progress-bar');
            let remainingTime = duration;
            let startTime = Date.now();
            let timerId = null;
            let isPaused = false;

            const updateProgressBar = () => {
                if (progressBar) {
                    progressBar.style.transition = `width ${remainingTime}ms linear`;
                    progressBar.style.width = '0%';
                }
            };

            const startTimer = () => {
                startTime = Date.now();
                updateProgressBar();
                timerId = setTimeout(() => {
                    dismissToast(toast);
                }, remainingTime);
            };

            const pauseTimer = () => {
                if (isPaused) return;
                isPaused = true;
                clearTimeout(timerId);
                const elapsed = Date.now() - startTime;
                remainingTime = Math.max(0, remainingTime - elapsed);
                if (progressBar) {
                    const computedWidth = window.getComputedStyle(progressBar).width;
                    progressBar.style.transition = 'none';
                    progressBar.style.width = computedWidth;
                }
            };

            const resumeTimer = () => {
                if (!isPaused || remainingTime <= 0) return;
                isPaused = false;
                startTimer();
            };

            toast.addEventListener('mouseenter', pauseTimer);
            toast.addEventListener('mouseleave', resumeTimer);

            // Initial timer start
            requestAnimationFrame(() => {
                startTimer();
            });
        }

        return toast;
    }

    function dismissToast(toast) {
        if (!toast || toast.classList.contains('mf-toast-hiding')) return;
        toast.classList.add('mf-toast-hiding');
        
        const removeToast = () => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        };

        toast.addEventListener('animationend', removeToast, { once: true });
        // Fallback safety timeout if animationend does not trigger
        setTimeout(removeToast, 400);
    }

    function clearAllToasts() {
        const container = document.getElementById('mfToastContainer');
        if (!container) return;
        const toasts = container.querySelectorAll('.mf-toast');
        toasts.forEach(toast => dismissToast(toast));
    }

    // Public MoneyFlowToast API
    window.MoneyFlowToast = {
        show: (options) => showToast(options),
        success: (message, title, duration) => showToast({ type: 'success', title: title || 'Success', message, duration }),
        error: (message, title, duration) => showToast({ type: 'error', title: title || 'Error', message, duration }),
        warning: (message, title, duration) => showToast({ type: 'warning', title: title || 'Warning', message, duration }),
        info: (message, title, duration) => showToast({ type: 'info', title: title || 'Information', message, duration }),
        dismiss: (toast) => dismissToast(toast),
        clearAll: () => clearAllToasts()
    };
})();

/**
 * Initializes toasts passed via TempData rendered in _ToastNotification partial
 */
function initServerToasts() {
    const serverToasts = document.querySelectorAll('.mf-server-toast');
    if (!serverToasts.length) return;

    serverToasts.forEach((el, index) => {
        const type = el.getAttribute('data-type') || 'info';
        const title = el.getAttribute('data-title') || '';
        const message = el.getAttribute('data-message') || '';

        if (message) {
            // Stagger slightly if multiple toasts are emitted simultaneously
            setTimeout(() => {
                if (window.MoneyFlowToast) {
                    window.MoneyFlowToast.show({
                        type,
                        title,
                        message
                    });
                }
            }, index * 150);
        }
        el.remove();
    });
}
