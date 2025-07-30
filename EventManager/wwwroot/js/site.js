document.addEventListener("DOMContentLoaded", function () {

    // Функция за работа с "Любими"
    function handleFavorites() {
        const favoriteButtons = document.querySelectorAll('.favorite-btn');
        let favorites = JSON.parse(localStorage.getItem('favoriteEvents')) || [];

        // 1. Оцветяваме сърцата на вече харесаните събития при зареждане
        favoriteButtons.forEach(button => {
            const eventId = button.getAttribute('data-event-id');
            if (favorites.includes(eventId)) {
                button.classList.add('active');
            }
        });

        // 2. Добавяме event listener за клик
        favoriteButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();
                const eventId = this.getAttribute('data-event-id');

                // Проверяваме дали събитието вече е в любими
                const index = favorites.indexOf(eventId);
                if (index > -1) {
                    // Ако е там, го махаме
                    favorites.splice(index, 1);
                    this.classList.remove('active');
                } else {
                    // Ако не е, го добавяме
                    favorites.push(eventId);
                    this.classList.add('active');
                }

                // 3. Запазваме обновения списък в LocalStorage
                localStorage.setItem('favoriteEvents', JSON.stringify(favorites));
            });
        });
    }

    // Извикваме функцията
    handleFavorites();

});