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
    // 3. Transactions Live Search & Filtering
    // -------------------------------------------------------------------------
    const txnSearchInput = document.getElementById('txnSearchInput');
    const txnTypeFilter = document.getElementById('txnTypeFilter');
    const txnStatusFilter = document.getElementById('txnStatusFilter');
    const txnRows = document.querySelectorAll('.mf-txn-data-row');
    const txnEmptyState = document.getElementById('txnEmptyState');
    const btnClearFilters = document.getElementById('btnClearTxnFilters');

    function filterTransactions() {
        if (!txnRows.length) return;

        const query = (txnSearchInput ? txnSearchInput.value : '').toLowerCase().trim();
        const selectedType = (txnTypeFilter ? txnTypeFilter.value : 'all').toLowerCase();
        const selectedStatus = (txnStatusFilter ? txnStatusFilter.value : 'all').toLowerCase();

        let visibleCount = 0;

        txnRows.forEach(row => {
            const text = row.textContent.toLowerCase();
            const rowType = (row.getAttribute('data-type') || '').toLowerCase();
            const rowStatus = (row.getAttribute('data-status') || '').toLowerCase();

            const matchesQuery = !query || text.includes(query);
            const matchesType = (selectedType === 'all' || rowType === selectedType);
            const matchesStatus = (selectedStatus === 'all' || rowStatus === selectedStatus);

            if (matchesQuery && matchesType && matchesStatus) {
                row.style.display = '';
                visibleCount++;
            } else {
                row.style.display = 'none';
            }
        });

        if (txnEmptyState) {
            txnEmptyState.classList.toggle('d-none', visibleCount > 0);
        }
    }

    if (txnSearchInput) txnSearchInput.addEventListener('input', filterTransactions);
    if (txnTypeFilter) txnTypeFilter.addEventListener('change', filterTransactions);
    if (txnStatusFilter) txnStatusFilter.addEventListener('change', filterTransactions);

    if (btnClearFilters) {
        btnClearFilters.addEventListener('click', function () {
            if (txnSearchInput) txnSearchInput.value = '';
            if (txnTypeFilter) txnTypeFilter.value = 'all';
            if (txnStatusFilter) txnStatusFilter.value = 'all';
            filterTransactions();
        });
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
    // 5. Customer Profile Edit Toggle & Password Validation
    // -------------------------------------------------------------------------
    const btnEditProfile = document.getElementById('btnEditProfile');
    const btnCancelEdit = document.getElementById('btnCancelEdit');
    const profileViewSection = document.getElementById('profileViewMode');
    const profileEditSection = document.getElementById('profileEditMode');
    const profileSuccessAlert = document.getElementById('profileUpdateSuccess');

    if (btnEditProfile && profileViewSection && profileEditSection) {
        btnEditProfile.addEventListener('click', function () {
            profileViewSection.classList.add('d-none');
            profileEditSection.classList.remove('d-none');
        });
    }

    if (btnCancelEdit && profileViewSection && profileEditSection) {
        btnCancelEdit.addEventListener('click', function () {
            profileEditSection.classList.add('d-none');
            profileViewSection.classList.remove('d-none');
        });
    }

    const profileEditForm = document.getElementById('profileEditForm');
    if (profileEditForm) {
        profileEditForm.addEventListener('submit', function (e) {
            e.preventDefault();
            profileEditSection.classList.add('d-none');
            profileViewSection.classList.remove('d-none');
            if (profileSuccessAlert) {
                profileSuccessAlert.classList.remove('d-none');
                setTimeout(() => profileSuccessAlert.classList.add('d-none'), 4000);
            }
        });
    }

    // Toggle Masked National ID
    const toggleNIdBtn = document.getElementById('btnToggleNId');
    const maskedNId = document.getElementById('maskedNationalId');
    if (toggleNIdBtn && maskedNId) {
        let isRevealed = false;
        const fullId = maskedNId.getAttribute('data-full-id') || '29508120194857';
        const maskedText = '••••••••••4857';

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

    if (newPassInput) {
        newPassInput.addEventListener('input', function () {
            const val = this.value;
            if (reqLength) reqLength.classList.toggle('text-success', val.length >= 8);
            if (reqNumber) reqNumber.classList.toggle('text-success', /\d/.test(val));
            if (reqSpecial) reqSpecial.classList.toggle('text-success', /[^A-Za-z0-9]/.test(val));
        });
    }
});
