/**
 * MoneyFlow — Employee & Staff Portal Client Interactions
 */

document.addEventListener('DOMContentLoaded', function () {
    // -------------------------------------------------------------------------
    // 1. Mobile Sidebar Toggle & Offcanvas Handling
    // -------------------------------------------------------------------------
    const sidebar = document.getElementById('mfEmployeeSidebar') || document.getElementById('mfPortalSidebar');
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
    // 2. Global Omnisearch Shortcut (Ctrl+K or Cmd+K)
    // -------------------------------------------------------------------------
    const omnisearchInput = document.getElementById('mfOmnisearchInput');
    window.addEventListener('keydown', function (e) {
        if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 'k') {
            e.preventDefault();
            if (omnisearchInput) {
                omnisearchInput.focus();
                omnisearchInput.select();
            }
        }
    });

    // -------------------------------------------------------------------------
    // 3. Open Account Modal with Searchable Customer Select
    // -------------------------------------------------------------------------
    const openAccountModalEl = document.getElementById('openAccountForCustomerModal');
    let openAccountModalInstance = null;
    if (openAccountModalEl) {
        openAccountModalInstance = new bootstrap.Modal(openAccountModalEl);
    }

    const inputSearchCustomer = document.getElementById('inputSearchCustomerModal');
    const customerDropdown = document.getElementById('customerSearchDropdown');
    const customerSearchList = document.getElementById('customerSearchList');
    const customerSearchNoMatch = document.getElementById('customerSearchNoMatch');
    const customerSearchBox = document.getElementById('customerSearchableSelectBox');
    const selectedBanner = document.getElementById('selectedCustomerBanner');
    const btnChangeCustomer = document.getElementById('btnChangeCustomer');
    const selectedCustIdVal = document.getElementById('selectedCustomerIdVal');

    function selectCustomer(id, name, email, natId) {
        if (selectedCustIdVal) selectedCustIdVal.value = id;

        const nameEl = document.getElementById('modalCustDisplayName');
        const idEl = document.getElementById('modalCustDisplayId');
        const natIdEl = document.getElementById('modalCustDisplayNatId');
        const avatarEl = document.getElementById('modalCustAvatar');

        if (nameEl) nameEl.textContent = name;
        if (idEl) idEl.textContent = id;
        if (natIdEl) natIdEl.textContent = natId || '--';
        if (avatarEl) {
            const parts = name.split(' ');
            avatarEl.textContent = (parts[0]?.[0] || 'C') + (parts[1]?.[0] || 'U');
        }

        if (customerSearchBox) customerSearchBox.style.display = 'none';
        if (customerDropdown) customerDropdown.style.display = 'none';
        if (selectedBanner) selectedBanner.style.display = 'flex';
        if (btnChangeCustomer) btnChangeCustomer.classList.remove('d-none');
    }

    function resetCustomerSelect() {
        if (selectedCustIdVal) selectedCustIdVal.value = '';
        if (inputSearchCustomer) inputSearchCustomer.value = '';
        if (customerSearchBox) customerSearchBox.style.display = 'block';
        if (selectedBanner) selectedBanner.style.display = 'none';
        if (btnChangeCustomer) btnChangeCustomer.classList.add('d-none');
        if (customerDropdown) customerDropdown.style.display = 'none';
    }

    if (btnChangeCustomer) {
        btnChangeCustomer.addEventListener('click', function () {
            resetCustomerSelect();
            if (inputSearchCustomer) inputSearchCustomer.focus();
        });
    }

    // Top general "+ Open Bank Account" button opens with fresh search
    const btnGeneralOpenAcc = document.getElementById('btnOpenGeneralAccountModal') || document.getElementById('btnOpenAccModalLedger');
    if (btnGeneralOpenAcc) {
        btnGeneralOpenAcc.addEventListener('click', function () {
            resetCustomerSelect();
            if (openAccountModalInstance) openAccountModalInstance.show();
        });
    }

    // Direct "+ Open Account" buttons from row pre-select the customer
    const openAccButtons = document.querySelectorAll('.btn-open-account-for-cust');
    openAccButtons.forEach(btn => {
        btn.addEventListener('click', function () {
            const custId = this.getAttribute('data-cust-id') || 'CUST-4091';
            const custName = this.getAttribute('data-cust-name') || 'Sarah Jenkins';
            const custEmail = this.getAttribute('data-cust-email') || '';
            const custNatId = this.getAttribute('data-cust-natid') || '';

            selectCustomer(custId, custName, custEmail, custNatId);

            if (openAccountModalInstance) {
                openAccountModalInstance.show();
            }
        });
    });

    // Live search typing in searchable select
    if (inputSearchCustomer) {
        inputSearchCustomer.addEventListener('input', function () {
            const query = this.value.toLowerCase().trim();
            if (!customerDropdown) return;

            customerDropdown.style.display = 'block';
            const items = customerSearchList?.querySelectorAll('.mf-searchable-item') || [];
            let matchCount = 0;

            items.forEach(item => {
                const text = item.innerText.toLowerCase();
                if (!query || text.includes(query)) {
                    item.style.display = 'flex';
                    matchCount++;
                } else {
                    item.style.display = 'none';
                }
            });

            if (customerSearchNoMatch) {
                customerSearchNoMatch.style.display = (matchCount === 0) ? 'block' : 'none';
            }
        });

        inputSearchCustomer.addEventListener('focus', function () {
            if (customerDropdown) {
                customerDropdown.style.display = 'block';
                this.dispatchEvent(new Event('input'));
            }
        });
    }

    // Click on item in searchable dropdown
    if (customerSearchList) {
        customerSearchList.addEventListener('click', function (e) {
            const item = e.target.closest('.mf-searchable-item');
            if (!item) return;

            const id = item.getAttribute('data-cust-id') || '';
            const name = item.getAttribute('data-cust-name') || '';
            const email = item.getAttribute('data-cust-email') || '';
            const natId = item.getAttribute('data-cust-natid') || '';

            selectCustomer(id, name, email, natId);
        });
    }

    // Close dropdown on outside click
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.mf-searchable-select-wrap')) {
            if (customerDropdown) customerDropdown.style.display = 'none';
        }
    });

    // Confirm & Issue Account
    const btnConfirmOpenAccount = document.getElementById('btnConfirmOpenAccount');
    if (btnConfirmOpenAccount) {
        btnConfirmOpenAccount.addEventListener('click', function () {
            const custId = selectedCustIdVal?.value;
            const custName = document.getElementById('modalCustDisplayName')?.textContent || 'Customer';

            if (!custId && selectedBanner?.style.display === 'none') {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('Please select a customer first.');
                return;
            }

            const acctType = document.getElementById('modalAccountTypeSelect')?.value || 'Current';
            const deposit = parseFloat(document.getElementById('modalInitialDepositInput')?.value || '0');
            const newAccNum = 'MF-' + Math.floor(100000 + Math.random() * 900000);

            if (openAccountModalInstance) {
                openAccountModalInstance.hide();
            }

            if (window.MoneyFlowToast) {
                let msg = 'New ' + acctType + ' Account (#' + newAccNum + ') opened for ' + custName + '.';
                if (deposit > 0) {
                    msg += ' Initial deposit $' + deposit.toFixed(2) + ' credited.';
                }
                window.MoneyFlowToast.success(msg);
            }
        });
    }

    // -------------------------------------------------------------------------
    // 4. Teller Cash Operations Interactive Wizard
    // -------------------------------------------------------------------------
    const opsAccountInput = document.getElementById('opsAccountInput');
    const btnLookupAccount = document.getElementById('btnLookupAccount');
    const opsLookupResult = document.getElementById('opsLookupResult');
    const opsAmountInput = document.getElementById('opsAmountInput');
    const opsTotalDisplay = document.getElementById('opsTotalDisplay');
    const btnSubmitOps = document.getElementById('btnSubmitOps');
    const receiptModalEl = document.getElementById('tellerReceiptModal');

    // Quick denomination chips
    const opsChips = document.querySelectorAll('.mf-ops-chip');
    opsChips.forEach(chip => {
        chip.addEventListener('click', function () {
            const val = parseFloat(this.getAttribute('data-val') || '0');
            if (opsAmountInput) {
                let current = parseFloat(opsAmountInput.value) || 0;
                opsAmountInput.value = (current + val).toFixed(2);
                opsAmountInput.dispatchEvent(new Event('input'));
            }
        });
    });

    // Mock accounts lookup database conforming strictly to AccountType and AccountStatus
    const mockAccounts = {
        'MF-100234': { name: 'Sarah Jenkins', type: 'Current Account', balance: 14520.50, status: 'Active', id: 'NAT-883921' },
        'MF-100876': { name: 'David Miller', type: 'Savings Account', balance: 52400.00, status: 'Active', id: 'NAT-450123' },
        'MF-100912': { name: 'Elena Rostova', type: 'Current Account', balance: 128900.75, status: 'Active', id: 'NAT-994821' },
        'MF-100445': { name: 'Marcus Vance', type: 'Savings Account', balance: 340.20, status: 'Suspended', id: 'NAT-223109' }
    };

    function lookupAccount() {
        if (!opsAccountInput) return;
        const acct = opsAccountInput.value.trim().toUpperCase();
        if (!acct) {
            if (window.MoneyFlowToast) window.MoneyFlowToast.warning('Please enter an account number to lookup');
            return;
        }

        const data = mockAccounts[acct] || {
            name: 'Customer Account (' + acct + ')',
            type: 'Current Account',
            balance: 8750.00,
            status: 'Active',
            id: 'NAT-' + Math.floor(100000 + Math.random() * 900000)
        };

        if (opsLookupResult) {
            opsLookupResult.classList.remove('d-none');
            const nameEl = document.getElementById('lookupCustName');
            const typeEl = document.getElementById('lookupAcctType');
            const balEl = document.getElementById('lookupAcctBal');
            const statusEl = document.getElementById('lookupAcctStatus');
            const avatarEl = document.getElementById('lookupAvatarText');

            if (nameEl) nameEl.textContent = data.name;
            if (typeEl) typeEl.textContent = data.type + ' • ' + acct;
            if (balEl) balEl.textContent = '$' + data.balance.toLocaleString('en-US', { minimumFractionDigits: 2 });
            if (avatarEl) {
                const parts = data.name.split(' ');
                avatarEl.textContent = (parts[0]?.[0] || 'C') + (parts[1]?.[0] || 'A');
            }
            if (statusEl) {
                if (data.status === 'Suspended') {
                    statusEl.className = 'badge bg-danger-subtle text-danger fw-bold';
                    statusEl.textContent = 'Suspended';
                } else {
                    statusEl.className = 'badge bg-success-subtle text-success fw-bold';
                    statusEl.textContent = 'Active';
                }
            }

            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.success('Account found: ' + data.name);
            }
        }
    }

    if (btnLookupAccount) btnLookupAccount.addEventListener('click', lookupAccount);
    if (opsAccountInput) {
        opsAccountInput.addEventListener('keydown', function(e) {
            if (e.key === 'Enter') {
                e.preventDefault();
                lookupAccount();
            }
        });
    }

    // Amount calculations
    if (opsAmountInput) {
        opsAmountInput.addEventListener('input', function () {
            const amount = parseFloat(this.value) || 0;
            if (opsTotalDisplay) opsTotalDisplay.textContent = '$' + amount.toLocaleString('en-US', { minimumFractionDigits: 2 });
        });
    }

    // Process & Generate Receipt Modal
    if (btnSubmitOps) {
        btnSubmitOps.addEventListener('click', function () {
            const amount = parseFloat(opsAmountInput?.value || '0');
            const acct = opsAccountInput?.value || 'MF-100234';
            const isWithdraw = document.getElementById('pills-withdraw-tab')?.classList.contains('active');
            const opType = isWithdraw ? 'Withdrawal' : 'Deposit';

            if (amount <= 0) {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('Please enter a valid amount');
                return;
            }

            // Fill receipt details
            const refCode = 'MF-' + Math.floor(10000000 + Math.random() * 90000000);
            const receiptRef = document.getElementById('receiptRefCode');
            const receiptType = document.getElementById('receiptOpType');
            const receiptAcct = document.getElementById('receiptAcctNum');
            const receiptAmt = document.getElementById('receiptTotalAmt');
            const receiptTime = document.getElementById('receiptTimestamp');

            if (receiptRef) receiptRef.textContent = refCode;
            if (receiptType) receiptType.textContent = opType;
            if (receiptAcct) receiptAcct.textContent = acct;
            if (receiptAmt) receiptAmt.textContent = '$' + amount.toLocaleString('en-US', { minimumFractionDigits: 2 });
            if (receiptTime) receiptTime.textContent = new Date().toLocaleString();

            if (receiptModalEl) {
                const modal = new bootstrap.Modal(receiptModalEl);
                modal.show();
            }

            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.success(opType + ' executed successfully!');
            }
        });
    }

    // -------------------------------------------------------------------------
    // 5. Live Table Search & Filter Helper strictly conforming to Enum attributes
    // -------------------------------------------------------------------------
    function setupTableFiltering(searchInputId, filterSelectId, tableBodyId, statusFilterId) {
        const searchInput = document.getElementById(searchInputId);
        const typeFilter = filterSelectId ? document.getElementById(filterSelectId) : null;
        const statusFilter = statusFilterId ? document.getElementById(statusFilterId) : null;
        const tableBody = document.getElementById(tableBodyId);

        if (!tableBody) return;

        function applyFilter() {
            const query = (searchInput?.value || '').toLowerCase().trim();
            const selectedType = (typeFilter?.value || '').toLowerCase();
            const selectedStatus = (statusFilter?.value || '').toLowerCase();

            const rows = tableBody.querySelectorAll('tr[data-filter-item="true"]');
            let visibleCount = 0;

            rows.forEach(row => {
                const text = row.innerText.toLowerCase();
                const rowType = (row.getAttribute('data-type') || '').toLowerCase();
                const rowStatus = (row.getAttribute('data-status') || '').toLowerCase();

                const matchQuery = !query || text.includes(query);
                const matchType = !selectedType || rowType === selectedType;
                const matchStatus = !selectedStatus || rowStatus === selectedStatus;

                if (matchQuery && matchType && matchStatus) {
                    row.style.display = '';
                    visibleCount++;
                } else {
                    row.style.display = 'none';
                }
            });

            const noResultsRow = tableBody.querySelector('.mf-no-filter-results');
            if (noResultsRow) {
                noResultsRow.style.display = (visibleCount === 0) ? '' : 'none';
            }
        }

        if (searchInput) searchInput.addEventListener('input', applyFilter);
        if (typeFilter) typeFilter.addEventListener('change', applyFilter);
        if (statusFilter) statusFilter.addEventListener('change', applyFilter);

        // Reset button
        const resetBtn = document.getElementById('btnClear' + searchInputId);
        if (resetBtn) {
            resetBtn.addEventListener('click', function () {
                if (searchInput) searchInput.value = '';
                if (typeFilter) typeFilter.value = '';
                if (statusFilter) statusFilter.value = '';
                applyFilter();
            });
        }
    }

    setupTableFiltering('empCustomerSearch', null, 'empCustomerTableBody', 'empCustomerStatusFilter');

    // -------------------------------------------------------------------------
    // 5c. Employee Accounts Server-Side Search & Filters
    // -------------------------------------------------------------------------
    const empAccountSearch = document.getElementById('empAccountSearch');
    const empAccountTypeFilter = document.getElementById('empAccountTypeFilter');
    const empAccountStatusFilter = document.getElementById('empAccountStatusFilter');
    const btnClearEmpAccountSearch = document.getElementById('btnClearempAccountSearch');

    if (empAccountSearch || empAccountTypeFilter || empAccountStatusFilter) {
        let debounceTimer;

        // Restore focus and cursor at end of input if search parameter exists
        if (empAccountSearch && empAccountSearch.value) {
            empAccountSearch.focus();
            empAccountSearch.setSelectionRange(empAccountSearch.value.length, empAccountSearch.value.length);
        }

        function applyEmpAccountFilters() {
            const url = new URL(window.location.origin + window.location.pathname);

            // Search
            const query = empAccountSearch ? empAccountSearch.value.trim() : '';
            if (query) {
                url.searchParams.set('Search', query);
            }

            // Account Type
            const type = empAccountTypeFilter ? empAccountTypeFilter.value : '';
            if (type) {
                url.searchParams.set('AccountType', type);
            }

            // Status
            const status = empAccountStatusFilter ? empAccountStatusFilter.value : '';
            if (status) {
                url.searchParams.set('Status', status);
            }

            // Reset page number to 1 on any filter or search update
            url.searchParams.set('page', '1');

            window.location.href = url.toString();
        }

        // Debounce search input (500ms)
        if (empAccountSearch) {
            empAccountSearch.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyEmpAccountFilters, 500);
            });
        }

        // Account type filter auto-trigger
        if (empAccountTypeFilter) {
            empAccountTypeFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyEmpAccountFilters();
            });
        }

        // Status filter auto-trigger
        if (empAccountStatusFilter) {
            empAccountStatusFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyEmpAccountFilters();
            });
        }

        // Reset / Clear filters button
        if (btnClearEmpAccountSearch) {
            btnClearEmpAccountSearch.addEventListener('click', function () {
                clearTimeout(debounceTimer);
                if (empAccountSearch) empAccountSearch.value = '';
                if (empAccountTypeFilter) empAccountTypeFilter.value = '';
                if (empAccountStatusFilter) empAccountStatusFilter.value = '';
                applyEmpAccountFilters();
            });
        }
    }

    // -------------------------------------------------------------------------
    // 5b. Employee Transactions Server-Side Search & Filters
    // -------------------------------------------------------------------------
    const empTxnSearch = document.getElementById('empTxnSearch');
    const empTxnTypeFilter = document.getElementById('empTxnTypeFilter');
    const empTxnStatusFilter = document.getElementById('empTxnStatusFilter');
    const btnClearEmpTxnSearch = document.getElementById('btnClearempTxnSearch');

    if (empTxnSearch || empTxnTypeFilter || empTxnStatusFilter) {
        let debounceTimer;

        // Restore focus and cursor at end of input if search parameter exists
        if (empTxnSearch && empTxnSearch.value) {
            empTxnSearch.focus();
            empTxnSearch.setSelectionRange(empTxnSearch.value.length, empTxnSearch.value.length);
        }

        function applyEmpTransactionFilters() {
            const url = new URL(window.location.origin + window.location.pathname);

            // Search
            const query = empTxnSearch ? empTxnSearch.value.trim() : '';
            if (query) {
                url.searchParams.set('Search', query);
            }

            // Transaction Type
            const type = empTxnTypeFilter ? empTxnTypeFilter.value : '';
            if (type) {
                url.searchParams.set('TransactionType', type);
            }

            // Status
            const status = empTxnStatusFilter ? empTxnStatusFilter.value : '';
            if (status) {
                url.searchParams.set('Status', status);
            }

            // Reset page number to 1 on any filter or search update
            url.searchParams.set('page', '1');

            window.location.href = url.toString();
        }

        // Debounce search input (500ms)
        if (empTxnSearch) {
            empTxnSearch.addEventListener('input', function () {
                clearTimeout(debounceTimer);
                debounceTimer = setTimeout(applyEmpTransactionFilters, 500);
            });
        }

        // Type filter auto-trigger
        if (empTxnTypeFilter) {
            empTxnTypeFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyEmpTransactionFilters();
            });
        }

        // Status filter auto-trigger
        if (empTxnStatusFilter) {
            empTxnStatusFilter.addEventListener('change', function () {
                clearTimeout(debounceTimer);
                applyEmpTransactionFilters();
            });
        }

        // Reset / Clear filters button
        if (btnClearEmpTxnSearch) {
            btnClearEmpTxnSearch.addEventListener('click', function () {
                clearTimeout(debounceTimer);
                if (empTxnSearch) empTxnSearch.value = '';
                if (empTxnTypeFilter) empTxnTypeFilter.value = '';
                if (empTxnStatusFilter) empTxnStatusFilter.value = '';
                applyEmpTransactionFilters();
            });
        }
    }


    // -------------------------------------------------------------------------
    // 6. Transaction Details Modal Click Handler
    // -------------------------------------------------------------------------
    const txnDetailModalEl = document.getElementById('txnDetailModal');
    if (txnDetailModalEl) {
        txnDetailModalEl.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget;
            if (!button) return;

            const ref = button.getAttribute('data-ref') || 'TXN-000000';
            const time = button.getAttribute('data-time') || '--';
            const type = button.getAttribute('data-type') || 'Transfer';
            const desc = button.getAttribute('data-desc') || 'Transaction';
            const amount = button.getAttribute('data-amount') || '$0.00';
            const status = button.getAttribute('data-status') || 'Completed';
            const sender = button.getAttribute('data-sender') || '--';
            const senderOwner = button.getAttribute('data-sender-owner') || '';
            const receiver = button.getAttribute('data-receiver') || '--';
            const receiverOwner = button.getAttribute('data-receiver-owner') || '';
            const channel = button.getAttribute('data-channel') || 'Teller Counter';

            const modalRef = txnDetailModalEl.querySelector('#modalTxnRef');
            const modalTime = txnDetailModalEl.querySelector('#modalTxnTime');
            const modalTypeBadge = txnDetailModalEl.querySelector('#modalTxnTypeBadge') || txnDetailModalEl.querySelector('#modalTxnType');
            const modalStatusBadge = txnDetailModalEl.querySelector('#modalTxnStatusBadge') || txnDetailModalEl.querySelector('#modalTxnStatus');
            const modalDesc = txnDetailModalEl.querySelector('#modalTxnDesc');
            const modalAmount = txnDetailModalEl.querySelector('#modalTxnAmount');
            const modalSender = txnDetailModalEl.querySelector('#modalTxnSender');
            const modalSenderOwner = txnDetailModalEl.querySelector('#modalTxnSenderOwner');
            const modalReceiver = txnDetailModalEl.querySelector('#modalTxnReceiver');
            const modalReceiverOwner = txnDetailModalEl.querySelector('#modalTxnReceiverOwner');
            const modalChannel = txnDetailModalEl.querySelector('#modalTxnChannel');

            if (modalRef) modalRef.textContent = ref;
            if (modalTime) modalTime.textContent = time;
            if (modalDesc) modalDesc.textContent = desc;
            
            if (modalAmount) {
                modalAmount.textContent = amount;
                if (type.toLowerCase() === 'deposit' || amount.startsWith('+')) {
                    modalAmount.className = 'display-6 fw-bold mb-2 text-success';
                } else {
                    modalAmount.className = 'display-6 fw-bold mb-2 text-dark';
                }
            }

            if (modalTypeBadge) {
                modalTypeBadge.textContent = type;
                if (type.toLowerCase() === 'deposit') {
                    modalTypeBadge.className = 'badge bg-success-subtle text-success px-3 py-2 fs-6';
                } else if (type.toLowerCase() === 'withdrawal') {
                    modalTypeBadge.className = 'badge bg-warning-subtle text-warning-emphasis px-3 py-2 fs-6';
                } else {
                    modalTypeBadge.className = 'badge bg-primary-subtle text-primary px-3 py-2 fs-6';
                }
            }

            if (modalStatusBadge) {
                modalStatusBadge.textContent = status;
                if (status.toLowerCase() === 'completed') {
                    modalStatusBadge.className = 'badge bg-success text-white px-3 py-2 fs-6';
                } else if (status.toLowerCase() === 'pending') {
                    modalStatusBadge.className = 'badge bg-warning-subtle text-warning-emphasis px-3 py-2 fs-6';
                } else {
                    modalStatusBadge.className = 'badge bg-danger-subtle text-danger px-3 py-2 fs-6';
                }
            }

            if (modalSender) modalSender.textContent = sender;
            if (modalSenderOwner) {
                if (senderOwner && senderOwner !== '--') {
                    modalSenderOwner.textContent = 'Owner: ' + senderOwner;
                    modalSenderOwner.style.display = 'block';
                } else {
                    modalSenderOwner.textContent = '';
                    modalSenderOwner.style.display = 'none';
                }
            }

            if (modalReceiver) modalReceiver.textContent = receiver;
            if (modalReceiverOwner) {
                if (receiverOwner && receiverOwner !== '--') {
                    modalReceiverOwner.textContent = 'Owner: ' + receiverOwner;
                    modalReceiverOwner.style.display = 'block';
                } else {
                    modalReceiverOwner.textContent = '';
                    modalReceiverOwner.style.display = 'none';
                }
            }

            if (modalChannel) modalChannel.textContent = channel;
        });
    }

    // -------------------------------------------------------------------------
    // 7. Employee Profile Password Live Validation & Update
    // -------------------------------------------------------------------------
    const empNewPassword = document.getElementById('empNewPassword');
    if (empNewPassword) {
        empNewPassword.addEventListener('input', function () {
            const val = this.value;

            function updateBadge(id, valid, text) {
                const el = document.getElementById(id);
                if (!el) return;
                if (valid) {
                    el.className = 'text-success fw-bold';
                    el.innerHTML = '<span class="me-1">✓</span> ' + text;
                } else {
                    el.className = 'text-muted';
                    el.innerHTML = '<span class="me-1">✕</span> ' + text;
                }
            }

            updateBadge('emp-req-len', val.length >= 8, 'At least 8 characters');
            updateBadge('emp-req-num', /[0-9]/.test(val), 'At least one number (0-9)');
            updateBadge('emp-req-special', /[!@#$%^&*]/.test(val), 'At least one special character (!@#$%^&*)');
            updateBadge('emp-req-upper', /[A-Z]/.test(val), 'At least one uppercase letter (A-Z)');
            updateBadge('emp-req-lower', /[a-z]/.test(val), 'At least one lowercase letter (a-z)');
        });
    }

    const btnUpdateEmpPassword = document.getElementById('btnUpdateEmpPassword');
    if (btnUpdateEmpPassword) {
        btnUpdateEmpPassword.addEventListener('click', function () {
            const current = document.getElementById('empCurrentPassword')?.value;
            const newPass = document.getElementById('empNewPassword')?.value;
            const confirm = document.getElementById('empConfirmPassword')?.value;

            if (!current || !newPass || !confirm) {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('Please fill in all password fields.');
                return;
            }

            if (newPass !== confirm) {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('New password and confirm password do not match.');
                return;
            }

            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.success('Employee credentials updated successfully.');
                document.getElementById('empChangePasswordForm')?.reset();
            }
        });
    }

    const btnSaveEmpInfo = document.getElementById('btnSaveEmpInfo');
    if (btnSaveEmpInfo) {
        btnSaveEmpInfo.addEventListener('click', function () {
            const firstName = document.getElementById('empEditFirstName')?.value || 'Staff';
            const lastName = document.getElementById('empEditLastName')?.value || 'Officer';
            const dob = document.getElementById('empEditDOB')?.value || '1990-05-12';
            const phone = document.getElementById('empEditPhone')?.value || '+1 (555) 019-4412';
            const address = document.getElementById('empEditAddress')?.value || '742 Evergreen Terrace, Springfield, OR';

            const dispFirst = document.getElementById('dispEmpFirstName');
            const dispLast = document.getElementById('dispEmpLastName');
            const dispDob = document.getElementById('dispEmpDOB');
            const dispPh = document.getElementById('dispEmpPhone');
            const dispAddr = document.getElementById('dispEmpAddress');

            if (dispFirst) dispFirst.textContent = firstName;
            if (dispLast) dispLast.textContent = lastName;
            if (dispDob) dispDob.textContent = dob;
            if (dispPh) dispPh.textContent = phone;
            if (dispAddr) dispAddr.textContent = address;

            const modalEl = document.getElementById('editEmployeeProfileModal');
            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
            }
            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.success('Employee personal information updated.');
            }
        });
    }

    // -------------------------------------------------------------------------
    // 8. Account Status Modification Modal
    // -------------------------------------------------------------------------
    const freezeConfirmBtn = document.getElementById('btnConfirmFreezeAccount');
    if (freezeConfirmBtn) {
        freezeConfirmBtn.addEventListener('click', function () {
            const acctNum = this.getAttribute('data-target-acct') || 'the selected account';
            const targetStatus = document.getElementById('accountStatusTargetSelect')?.value || 'Suspended';
            const modalEl = document.getElementById('freezeAccountModal');
            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
            }
            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.info('Account ' + acctNum + ' status updated to ' + targetStatus + '.');
            }
        });
    }

    // -------------------------------------------------------------------------
    // 9. Register New Customer Modal Interactive Validation & Submission
    // -------------------------------------------------------------------------
    // Eye show/hide toggle for modal password inputs
    const modalEyeBtns = document.querySelectorAll('.mf-modal-password-toggle-btn');
    modalEyeBtns.forEach(btn => {
        btn.addEventListener('click', function () {
            const targetId = this.getAttribute('data-target');
            const input = document.getElementById(targetId);
            if (!input) return;

            const isPassword = input.type === 'password';
            input.type = isPassword ? 'text' : 'password';

            const eyeOpen = this.querySelector('.mf-eye-open');
            const eyeClosed = this.querySelector('.mf-eye-closed');

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

    // Password live checklist & strength meter for register customer modal
    const modalRegPassword = document.getElementById('modalRegPassword');
    if (modalRegPassword) {
        modalRegPassword.addEventListener('input', function () {
            const val = this.value;

            function updateReq(id, valid, text) {
                const el = document.getElementById(id);
                if (!el) return;
                if (valid) {
                    el.className = 'text-success fw-bold';
                    el.innerHTML = '<span class="me-1">✓</span> ' + text;
                } else {
                    el.className = 'text-muted';
                    el.innerHTML = '<span class="me-1">✕</span> ' + text;
                }
            }

            const hasLen = val.length >= 8;
            const hasUpper = /[A-Z]/.test(val);
            const hasLower = /[a-z]/.test(val);
            const hasNum = /[0-9]/.test(val);
            const hasSpecial = /[!@#$%^&*]/.test(val);

            updateReq('modal-req-len', hasLen, 'At least 8 characters');
            updateReq('modal-req-upper', hasUpper, 'One uppercase letter (A-Z)');
            updateReq('modal-req-lower', hasLower, 'One lowercase letter (a-z)');
            updateReq('modal-req-num', hasNum, 'One number (0-9)');
            updateReq('modal-req-special', hasSpecial, 'One special character (!@#$%^&*)');

            // Strength calculation
            let score = 0;
            if (hasLen) score++;
            if (hasUpper && hasLower) score++;
            if (hasNum && hasSpecial) score++;

            const bar1 = document.getElementById('modal-str-bar-1');
            const bar2 = document.getElementById('modal-str-bar-2');
            const bar3 = document.getElementById('modal-str-bar-3');
            const strLabel = document.getElementById('modal-str-label');

            if (bar1 && bar2 && bar3 && strLabel) {
                if (score === 0) {
                    bar1.style.backgroundColor = '#cbd5e1';
                    bar2.style.backgroundColor = '#cbd5e1';
                    bar3.style.backgroundColor = '#cbd5e1';
                    strLabel.textContent = 'Password strength: Weak';
                    strLabel.className = 'small text-muted fw-semibold';
                } else if (score === 1) {
                    bar1.style.backgroundColor = '#ef4444';
                    bar2.style.backgroundColor = '#cbd5e1';
                    bar3.style.backgroundColor = '#cbd5e1';
                    strLabel.textContent = 'Password strength: Weak';
                    strLabel.className = 'small text-danger fw-semibold';
                } else if (score === 2) {
                    bar1.style.backgroundColor = '#f59e0b';
                    bar2.style.backgroundColor = '#f59e0b';
                    bar3.style.backgroundColor = '#cbd5e1';
                    strLabel.textContent = 'Password strength: Medium';
                    strLabel.className = 'small text-warning-emphasis fw-semibold';
                } else if (score === 3) {
                    bar1.style.backgroundColor = '#10b981';
                    bar2.style.backgroundColor = '#10b981';
                    bar3.style.backgroundColor = '#10b981';
                    strLabel.textContent = 'Password strength: Strong';
                    strLabel.className = 'small text-success fw-semibold';
                }
            }
        });
    }

    const btnSaveNewCustomer = document.getElementById('btnSaveNewCustomer');
    if (btnSaveNewCustomer) {
        btnSaveNewCustomer.addEventListener('click', function () {
            const form = document.getElementById('newCustomerForm');
            if (form && !form.checkValidity()) {
                form.reportValidity();
                return;
            }

            const pass = document.getElementById('modalRegPassword')?.value || '';
            const confirmPass = document.getElementById('modalRegConfirmPassword')?.value || '';

            if (pass !== confirmPass) {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('Password and Confirm Password do not match.');
                return;
            }

            const firstName = document.getElementById('modalRegFirstName')?.value || 'Customer';
            const lastName = document.getElementById('modalRegLastName')?.value || '';
            const natId = document.getElementById('modalRegNationalId')?.value || '';

            if (natId && !/^\d{14}$/.test(natId)) {
                if (window.MoneyFlowToast) window.MoneyFlowToast.error('National ID must contain exactly 14 digits.');
                return;
            }

            const modalEl = document.getElementById('newCustomerModal');
            if (modalEl) {
                const modal = bootstrap.Modal.getInstance(modalEl);
                if (modal) modal.hide();
            }

            if (window.MoneyFlowToast) {
                window.MoneyFlowToast.success('Customer account for ' + firstName + ' ' + lastName + ' registered successfully.');
                form?.reset();
            }
        });
    }
});
