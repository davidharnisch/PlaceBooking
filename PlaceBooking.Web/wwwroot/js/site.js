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

// Rooms map (Map.cshtml): replace <img> with inline SVG and attach click handlers
(() => {
    function initRoomsMap(root = document) {
        const mapHost = root.querySelector('[data-room-map].pb-rooms-map') || root.querySelector('[data-room-map]');
        if (!mapHost) return;

        const img = mapHost.querySelector('img');
        if (!img) return;

        const isReadOnly = (mapHost.getAttribute('data-read-only') || '').toLowerCase() === 'true';
        const roomBaseUrl = mapHost.getAttribute('data-room-base-url');
        if (!roomBaseUrl) return;

        fetch(img.src)
            .then(r => r.text())
            .then(svgText => {
                mapHost.innerHTML = svgText;

                const svg = mapHost.querySelector('svg');
                if (!svg) return;

                svg.classList.add('pb-rooms-map-svg');

                const rooms = svg.querySelectorAll('[id^="room-"]');
                rooms.forEach(room => {
                    if (!room || !room.id) return;

                    room.classList.add('pb-room');
                    if (isReadOnly) room.classList.add('pb-room--readonly');

                    room.addEventListener('click', () => {
                        const m = room.id.match(/room-(\d+)/i);
                        const roomId = m ? m[1] : room.id;
                        window.location.href = roomBaseUrl + '?roomId=' + encodeURIComponent(roomId);
                    });
                });
            })
            .catch(() => {
                // Keep the <img> fallback.
            });
    }

    window.initRoomsMap = initRoomsMap;
    document.addEventListener('DOMContentLoaded', function () { initRoomsMap(document); });
})();

// Seating map (Room.cshtml): load inline SVG, apply seat state, handle modal actions
(() => {
    function seatNumberFromLabel(label) {
        if (!label) return null;
        const m = String(label).match(/-(\d+)$/);
        return m ? m[1] : null;
    }

    function initRoomSeatingMap(root = document) {
        const host = root.querySelector('[data-room-map][data-svg-url][data-room-id]');
        if (!host) return;

        const svgUrl = host.getAttribute('data-svg-url');
        const roomId = host.getAttribute('data-room-id');
        if (!svgUrl || !roomId) return;

        const seatsJson = host.getAttribute('data-seats');
        if (!seatsJson) return;

        let seats;
        try { seats = JSON.parse(seatsJson); } catch { return; }

        const currentUserId = parseInt(host.getAttribute('data-current-user-id') || '0', 10);
        const isReadOnly = (host.getAttribute('data-read-only') || '').toLowerCase() === 'true';
        const selectedDate = host.getAttribute('data-selected-date') || '';
        const todayRaw = host.getAttribute('data-today') || '';
        const isSelectedDateNotInPast = (host.getAttribute('data-selected-date-not-in-past') || '').toLowerCase() === 'true';
        const selectedDateDisplay = host.getAttribute('data-selected-date-display') || '';

        function applySeatState(seatEl, seat) {
            const isMyBooking = seat.IsBooked && seat.BookedByUserId === currentUserId;
            const isPastDate = selectedDate && todayRaw ? selectedDate < todayRaw : false;
            const canCreateBooking = !isReadOnly && !isPastDate;
            const canCancelBooking = !isReadOnly && isSelectedDateNotInPast;

            seatEl.classList.add('pb-seat');
            seatEl.classList.toggle('pb-seat--free', !seat.IsBooked);
            seatEl.classList.toggle('pb-seat--mine', isMyBooking);
            seatEl.classList.toggle('pb-seat--busy', seat.IsBooked && !isMyBooking);
            seatEl.classList.toggle('pb-seat--disabled', (!canCreateBooking && !seat.IsBooked) || (seat.IsBooked && !isMyBooking));

            let tooltip = 'Volno';
            if (seat.IsBooked) {
                const bookedAt = seat.BookedAt ? new Date(seat.BookedAt) : null;
                const bookedAtText = bookedAt ? bookedAt.toLocaleString('cs-CZ') : '';
                tooltip = isMyBooking
                    ? `Moje rezervace${bookedAtText ? ' | Vytvořeno: ' + bookedAtText : ''}`
                    : `Obsazeno: ${seat.BookedBy || ''}${bookedAtText ? ' | Vytvořeno: ' + bookedAtText : ''}`;
            }

            seatEl.setAttribute('data-bs-toggle', 'tooltip');
            seatEl.setAttribute('data-bs-placement', 'top');
            seatEl.setAttribute('data-bs-title', tooltip);
        }

        fetch(svgUrl)
            .then(r => {
                if (!r.ok) throw new Error('Failed to load SVG: ' + r.status);
                return r.text();
            })
            .then(svgText => {
                host.innerHTML = svgText;

                const svg = host.querySelector('svg');
                if (!svg) return;
                svg.classList.add('pb-room-map-svg');

                const seatByNumber = new Map();
                for (const s of seats) {
                    const n = seatNumberFromLabel(s.Label);
                    if (n) seatByNumber.set(String(n), s);
                }

                const seatEls = svg.querySelectorAll('[id^="seat-"]');
                seatEls.forEach(seatEl => {
                    const id = seatEl.getAttribute('id') || '';
                    const m = id.match(/^seat-(\d+)-(\d+)$/);
                    if (!m) return;
                    const svgRoomId = m[1];
                    const seatNo = String(parseInt(m[2], 10));
                    if (svgRoomId !== String(roomId)) return;

                    const seat = seatByNumber.get(seatNo);
                    if (!seat) return;

                    let targetEl = seatEl;
                    if (seatEl.children && seatEl.children.length === 1) {
                        targetEl = seatEl.children[0];
                    }

                    applySeatState(targetEl, seat);

                    seatEl.addEventListener('click', () => {
                        const isMyBooking = seat.IsBooked && seat.BookedByUserId === currentUserId;
                        const isPastDate = selectedDate && todayRaw ? selectedDate < todayRaw : false;
                        const canCreateBooking = !isReadOnly && !isPastDate;
                        const canCancelBooking = !isReadOnly && isSelectedDateNotInPast;

                        const modalEl = document.getElementById('seatActionModal');
                        if (!modalEl || !window.bootstrap) return;

                        const titleEl = document.getElementById('seatActionModalTitle');
                        const bodyEl = document.getElementById('seatActionModalBody');
                        const createForm = document.getElementById('seatCreateForm');
                        const cancelForm = document.getElementById('seatCancelForm');
                        const createSeatId = document.getElementById('seatCreateSeatId');
                        const cancelBookingId = document.getElementById('seatCancelBookingId');

                        if (!titleEl || !bodyEl || !createForm || !cancelForm || !createSeatId || !cancelBookingId) return;

                        createForm.classList.add('d-none');
                        cancelForm.classList.add('d-none');

                        if (!seat.IsBooked && canCreateBooking) {
                            titleEl.textContent = 'Potvrzení rezervace';
                            bodyEl.innerHTML = `Opravdu chcete zabookovat místo <strong>${seat.Label}</strong> na den ${selectedDateDisplay}?`;
                            createSeatId.value = seat.Id;
                            createForm.classList.remove('d-none');
                            bootstrap.Modal.getOrCreateInstance(modalEl).show();
                        } else if (seat.IsBooked && isMyBooking && canCancelBooking && seat.BookingId) {
                            titleEl.textContent = 'Zrušit rezervaci';
                            bodyEl.innerHTML = `Opravdu chcete zrušit svou rezervaci místa <strong>${seat.Label}</strong> pro den ${selectedDateDisplay}?`;
                            cancelBookingId.value = seat.BookingId;
                            cancelForm.classList.remove('d-none');
                            bootstrap.Modal.getOrCreateInstance(modalEl).show();
                        }
                    });
                });

                if (window.bootstrap) {
                    svg.querySelectorAll('[data-bs-toggle="tooltip"]').forEach(el => {
                        bootstrap.Tooltip.getOrCreateInstance(el);
                    });
                }
            })
            .catch(err => {
                console.error('Failed to initialize seating SVG map', err);
            });
    }

    window.initRoomSeatingMap = initRoomSeatingMap;
    document.addEventListener('DOMContentLoaded', function () { initRoomSeatingMap(document); });
})();

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
