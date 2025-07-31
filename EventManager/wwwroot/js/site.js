document.addEventListener("DOMContentLoaded", function () {


    function handleFavorites() {
        const favoriteButtons = document.querySelectorAll('.favorite-btn');
        if (favoriteButtons.length === 0) return;

        let favorites = JSON.parse(localStorage.getItem('favoriteEvents')) || [];

        favoriteButtons.forEach(button => {
            const eventId = button.getAttribute('data-event-id');
            if (favorites.includes(eventId)) {
                button.classList.add('active');
            }
        });

        favoriteButtons.forEach(button => {
            button.addEventListener('click', function (e) {
                e.preventDefault();
                const eventId = this.getAttribute('data-event-id');
                const index = favorites.indexOf(eventId);

                if (index > -1) {
                    favorites.splice(index, 1);
                    this.classList.remove('active');
                } else {
                    favorites.push(eventId);
                    this.classList.add('active');
                }

                localStorage.setItem('favoriteEvents', JSON.stringify(favorites));

                if (window.location.pathname.toLowerCase().includes('/event/favorites')) {

                    setTimeout(() => {
                       
                        navigateToFavorites();
                    }, 200);
                }
            });
        });
    }

   
    function navigateToFavorites() {
        const favorites = JSON.parse(localStorage.getItem('favoriteEvents')) || [];
        if (favorites.length === 0) {
            window.location.href = '/Event/Favorites';
        } else {
            const queryString = favorites.map(id => `ids=${id}`).join('&');
            window.location.href = `/Event/Favorites?${queryString}`;
        }
    }

  
    handleFavorites();

    const favoritesLink = document.getElementById('favoritesLink');
    if (favoritesLink) {
        favoritesLink.addEventListener('click', function (e) {
            e.preventDefault();
            navigateToFavorites();
        });
    }
});