document.addEventListener('DOMContentLoaded', () => {
    // Ищем все модальные окна с классом .js-delete-modal
    const modals = document.querySelectorAll('[data-bs-toggle="modal"][data-bs-target^="#deleteModal"]');

    modals.forEach(modalTrigger => {
        const modalId = modalTrigger.getAttribute('data-bs-target'); // например, "#deleteModal"
        const modalEl = document.querySelector(modalId);

        if (!modalEl) return;

        // Проверка на Bootstrap (чтобы не ломалось, если его нет)
        if (typeof window === 'undefined' || typeof window.bootstrap === 'undefined') {
            console.warn('Bootstrap JS не найден! Удаление через модалку работать не будет.');
            return;
        }

        const myModal = new window.bootstrap.Modal(modalEl);

        modalEl.addEventListener('show.bs.modal', function (event) {
            const button = event.relatedTarget; // Кнопка, которая вызвала открытие

            if (!button) return;

            // Получаем ID и имя из data-атрибутов кнопки
            const itemId = button.getAttribute('data-item-id');
            const itemName = button.getAttribute('data-item-name');

            // Ищем поля внутри этой модалки
            const inputId = modalEl.querySelector('input[name="id"]');
            const spanName = modalEl.querySelector('#itemNameToDelete'); 
            const warning = modalEl.querySelector('#fkErrorWarning');

            // Заполняем
            if (inputId) {
                inputId.value = itemId;
            }
            if (spanName) {
                spanName.textContent = itemName;
            }
            if (warning) {
                warning.classList.add('d-none');
            }
        });
    });
});
