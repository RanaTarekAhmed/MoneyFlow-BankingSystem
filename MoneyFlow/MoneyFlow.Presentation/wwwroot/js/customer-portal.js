/**
 * MoneyFlow — Customer Portal Client Interactions
 */

document.addEventListener('DOMContentLoaded', function () {
    // -------------------------------------------------------------------------
    // 1. Mobile Sidebar Toggle & Offcanvas Handling
    // -------------------------------------------------------------------------
    const sidebar = document.getElementById('mfPortalSidebar');
    const backdrop = document.getElementById('mfSidebarBackdrop');
    const toggler = document.getElementById('mfSidebarToggle');
    const closeSidebarBtn = document.getElementById('mfSidebarClose');

    function openSidebar() {
        if (sidebar) sidebar.classList.add('show');
        if (backdrop) backdrop.classList.add('show');
        document.body.style.overflow = 'hidden';
    }

    function closeSidebar() {
        if (sidebar) sidebar.classList.remove('show');
        if (backdrop) backdrop.classList.remove('show');
        document.body.style.overflow = '';
    }

    if (toggler) toggler.addEventListener('click', openSidebar);
    if (closeSidebarBtn) closeSidebarBtn.addEventListener('click', closeSidebar);
    if (backdrop) backdrop.addEventListener('click', closeSidebar);

    window.addEventListener('resize', function () {
        if (window.innerWidth >= 992) {
            closeSidebar();
        }
    });

    // -------------------------------------------------------------------------
    // 2. Transfer Money Workflow Wizard
    // -------------------------------------------------------------------------
    const transferForm = document.getElementById('mfTransferForm');
    const fromAccountSelect = document.getElementById('fromAccountSelect');
    const toAccountInput = document.getElementById('toAccountInput');
    const amountInput = document.getElementById('transferAmountInput');
    const descInput = document.getElementById('transferDescriptionInput');
    const step1Container = document.getElementById('transferStep1');
    const step2Container = document.getElementById('transferStep2');
    const successCard = document.getElementById('transferSuccessCard');
    const reviewModalEl = document.getElementById('transferReviewModal');

    // Quick Amount Chips
    const amountChips = document.querySelectorAll('.mf-chip-btn');
    amountChips.forEach(chip => {
        chip.addEventListener('click', function () {
            const val = this.getAttribute('data-amount');
            if (amountInput) {
                amountInput.value = parseFloat(val).toFixed(2);
                amountInput.dispatchEvent(new Event('input'));
            }
        });
    });

    // Quick Recipient Selection
    const recipientBtns = document.querySelectorAll('.mf-quick-recipient-btn');
    recipientBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const acc = this.getAttribute('data-account');
            if (toAccountInput) {
                toAccountInput.value = acc;
                toAccountInput.dispatchEvent(new Event('input'));
            }
        });
    });

    // Swap Accounts Button
    const swapBtn = document.getElementById('mfSwapAccountsBtn');
    if (swapBtn && fromAccountSelect && toAccountInput) {
        swapBtn.addEventListener('click', function () {
            const temp = toAccountInput.value;
            toAccountInput.value = fromAccountSelect.value;
            // animate swap
            this.classList.add('rotate');
            setTimeout(() => this.classList.remove('rotate'), 300);
        });
    }

    // Review Transfer Button
    const reviewBtn = document.getElementById('btnReviewTransfer');
    if (reviewBtn) {
        reviewBtn.addEventListener('click', function () {
            // Validation
            const fromVal = fromAccountSelect ? fromAccountSelect.value : '';
            const toVal = toAccountInput ? toAccountInput.value.trim() : '';
            const amountVal = amountInput ? parseFloat(amountInput.value) : 0;

            if (!toVal) {
                alert('Please enter a recipient account number.');
                if (toAccountInput) toAccountInput.focus();
                return;
            }

            if (!amountVal || amountVal <= 0) {
                alert('Please enter a valid transfer amount greater than $0.00.');
                if (amountInput) amountInput.focus();
                return;
            }

            // Populate Review Modal
            const revFrom = document.getElementById('revFromAccount');
            const revTo = document.getElementById('revToAccount');
            const revAmount = document.getElementById('revAmount');
            const revTotal = document.getElementById('revTotal');
            const revDesc = document.getElementById('revDescription');

            if (revFrom && fromAccountSelect) {
                const selectedOption = fromAccountSelect.options[fromAccountSelect.selectedIndex];
                revFrom.textContent = selectedOption ? selectedOption.text : fromVal;
            }
            if (revTo) revTo.textContent = toVal;
            if (revAmount) revAmount.textContent = '$' + amountVal.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            if (revTotal) revTotal.textContent = '$' + amountVal.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            if (revDesc) revDesc.textContent = (descInput && descInput.value.trim()) ? descInput.value.trim() : 'No description provided';

            // Show Bootstrap Modal
            if (reviewModalEl && window.bootstrap) {
                const modal = bootstrap.Modal.getOrCreateInstance(reviewModalEl);
                modal.show();
            }
        });
    }

    // Confirm Transfer CTA inside Modal
    const confirmTransferBtn = document.getElementById('btnConfirmTransfer');
    if (confirmTransferBtn) {
        confirmTransferBtn.addEventListener('click', function () {
            // Hide modal
            if (reviewModalEl && window.bootstrap) {
                const modal = bootstrap.Modal.getInstance(reviewModalEl);
                if (modal) modal.hide();
            }

            // Show Success Receipt State
            if (transferForm) transferForm.classList.add('d-none');
            if (successCard) {
                successCard.classList.remove('d-none');
                // update receipt values
                const receiptAmount = document.getElementById('receiptAmount');
                const receiptTo = document.getElementById('receiptTo');
                const receiptTxn = document.getElementById('receiptTxnNumber');
                const receiptTime = document.getElementById('receiptTime');

                const amountVal = amountInput ? parseFloat(amountInput.value) : 0;
                if (receiptAmount) receiptAmount.textContent = '$' + amountVal.toLocaleString('en-US', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
                if (receiptTo && toAccountInput) receiptTo.textContent = toAccountInput.value;
                if (receiptTxn) receiptTxn.textContent = 'TXN-' + Math.floor(1000000 + Math.random() * 9000000);
                if (receiptTime) receiptTime.textContent = new Date().toLocaleString('en-US', { dateStyle: 'medium', timeStyle: 'short' });
                
                successCard.scrollIntoView({ behavior: 'smooth' });
            }
        });
    }

    // Reset Transfer (Make another transfer)
    const btnNewTransfer = document.getElementById('btnNewTransfer');
    if (btnNewTransfer) {
        btnNewTransfer.addEventListener('click', function () {
            if (transferForm) {
                transferForm.reset();
                transferForm.classList.remove('d-none');
            }
            if (successCard) successCard.classList.add('d-none');
        });
    }

    // -------------------------------------------------------------------------
    // 3. Transactions Server-Side Search & Filters
    // -------------------------------------------------------------------------
    const txnSearchInput = document.getElementById('txnSearchInput');
    const txnTypeFilter = document.getElementById('txnTypeFilter');
    const txnStatusFilter = document.getElementById('txnStatusFilter');
    const btnClearTxnFilters = document.getElementById('btnClearTxnFilters');

    if (txnSearchInput || txnTypeFilter || txnStatusFilter) {
        let debounceTimer;

        // Restore focus and cursor at end of input if search parameter exists
        if (txnSearchInput && txnSearchInput.value) {
            txnSearchInput.focus();
            txnSearchInput.setSelectionRange(txnSearchInput.value.length, txnSearchInput.value.length);
        }

        function applyTransactionFilters() {
            const url = new URL(window.location.origin + window.location.pathname);

            // Search
            const query = txnSearchInput ? txnSearchInput.value.trim() : '';
            if (query) {
                url.searchParams.set('Search', query);
            }

            // Transaction Type
            const type = txnTypeFilter ? txnTypeFilter.value : '';
            if (type) {
                url.searchParams.set('TransactionType', type);
            }

            // Status
            const status = txnStatusFilter ? txnStatusFilter.value : '';
            if (status) {
                url.searchParams.set('Status', status);
            }

            // Reset page number to 1 on any filter or search update
            url.searchParams.set('page', '1');

            window.location.href = url.toString();
        }

        // Debounce search input (500ms)
        if (txnSearchInput) {
            txnSearchInput.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyTransactionFilters, 500);
            });
        }

        // Type filter auto-trigger
        if (txnTypeFilter) {
            txnTypeFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyTransactionFilters();
            });
        }

        // Status filter auto-trigger
        if (txnStatusFilter) {
            txnStatusFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyTransactionFilters();
            });
        }

        // Reset / Clear filters button
        if (btnClearTxnFilters) {
            btnClearTxnFilters.addEventListener('click', function () {
                clearTimeout(debounceTimer);
                if (txnSearchInput) txnSearchInput.value = '';
                if (txnTypeFilter) txnTypeFilter.value = '';
                if (txnStatusFilter) txnStatusFilter.value = '';
                applyTransactionFilters();
            });
        }
    }

    // -------------------------------------------------------------------------
    // 4. Notifications Filter & Mark as Read
    // -------------------------------------------------------------------------
    const notifTabBtns = document.querySelectorAll('.mf-notif-tab');
    const notifItems = document.querySelectorAll('.mf-notification-item');
    const btnMarkAllRead = document.getElementById('btnMarkAllRead');

    notifTabBtns.forEach(tab => {
        tab.addEventListener('click', function () {
            notifTabBtns.forEach(t => t.classList.remove('active'));
            this.classList.add('active');

            const filter = this.getAttribute('data-filter');
            notifItems.forEach(item => {
                if (filter === 'all') {
                    item.style.display = '';
                } else if (filter === 'unread') {
                    item.style.display = item.classList.contains('unread') ? '' : 'none';
                } else {
                    item.style.display = item.getAttribute('data-category') === filter ? '' : 'none';
                }
            });
        });
    });

    if (btnMarkAllRead) {
        btnMarkAllRead.addEventListener('click', function () {
            notifItems.forEach(item => item.classList.remove('unread'));
            const notifBadge = document.querySelector('.mf-badge-dot');
            if (notifBadge) notifBadge.remove();
            const sidebarBadge = document.querySelector('.mf-nav-badge');
            if (sidebarBadge) sidebarBadge.remove();
        });
    }

    // -------------------------------------------------------------------------
    // 5. Customer Profile Edit Modal & Password Validation
    // -------------------------------------------------------------------------
    const editPersonalInfoModalEl = document.getElementById('editPersonalInfoModal');
    const editPersonalInfoForm = document.getElementById('editPersonalInfoForm');
    const profileSuccessAlert = document.getElementById('profileUpdateSuccess');

    // Toggle Masked National ID
    const toggleNIdBtn = document.getElementById('btnToggleNId');
    const maskedNId = document.getElementById('maskedNationalId');
    if (toggleNIdBtn && maskedNId) {
        let isRevealed = false;
        const fullId = maskedNId.getAttribute('data-full-id') || '';
        const lastFour = fullId.length >= 4 ? fullId.slice(-4) : '••••';
        const maskedText = `••••••••••${lastFour}`;

        toggleNIdBtn.addEventListener('click', function () {
            isRevealed = !isRevealed;
            maskedNId.textContent = isRevealed ? fullId : maskedText;
            this.textContent = isRevealed ? 'Hide' : 'Show';
        });
    }

    // Password live requirements validation
    const newPassInput = document.getElementById('newPasswordInput');
    const reqLength = document.getElementById('req-length');
    const reqNumber = document.getElementById('req-number');
    const reqSpecial = document.getElementById('req-special');
    const reqUpper = document.getElementById('req-upper');
    const reqLower = document.getElementById('req-lower');

    if (newPassInput) {
        newPassInput.addEventListener('input', function () {
            const val = this.value;
            if (reqLength) reqLength.classList.toggle('text-success', val.length >= 8);
            if (reqNumber) reqNumber.classList.toggle('text-success', /\d/.test(val));
            if (reqSpecial) reqSpecial.classList.toggle('text-success', /[^A-Za-z0-9]/.test(val));
            if (reqUpper) reqUpper.classList.toggle('text-success', /[A-Z]/.test(val));
            if (reqLower) reqLower.classList.toggle('text-success', /[a-z]/.test(val));
        });
    }
});


