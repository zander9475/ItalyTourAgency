// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

//const carousel = new bootstrap.Carousel('#myCarousel')

document.addEventListener('DOMContentLoaded', function () {
    // Initialize carousel
    const carousel = new bootstrap.Carousel('#videoCarousel', {
        interval: 8000,
        pause: false
    });

    // Handle video playback
    const videoCarousel = document.getElementById('videoCarousel');
    const videos = document.querySelectorAll('.carousel-item video');

    // Play first video
    videos[0].play();

    // Play video when slide changes
    videoCarousel.addEventListener('slid.bs.carousel', function () {
        const activeIndex = document.querySelector('.carousel-item.active').dataset('bs-interval');
        videos.forEach(v => v.pause());
        videos[activeIndex].play();
    });
});