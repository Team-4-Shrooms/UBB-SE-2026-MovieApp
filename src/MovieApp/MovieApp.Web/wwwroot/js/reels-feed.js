let globalMuted = true; // Start muted to allow autoplay

document.addEventListener("DOMContentLoaded", function () {
    const videos = document.querySelectorAll(".reel-item video");
    
    // Set initial mute state on all videos
    videos.forEach(video => {
        video.muted = globalMuted;
    });

    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            const video = entry.target;
            if (entry.isIntersecting) {
                // Apply global mute state to the video before playing
                video.muted = globalMuted;
                updateMuteButtons();
                
                // Play active video
                video.play().catch(err => {
                    console.log("Autoplay blocked or interrupted:", err);
                });
            } else {
                // Pause inactive video
                video.pause();
            }
        });
    }, {
        threshold: 0.6 // Trigger when 60% of the video is visible
    });

    videos.forEach(video => {
        observer.observe(video);
    });
    
    // Add click listener to videos to toggle play/pause or unmute
    videos.forEach(video => {
        video.addEventListener("click", function () {
            if (video.muted) {
                // If it was muted, unmute globally
                globalMuted = false;
                syncMuteStateAcrossVideos();
            } else {
                // Otherwise toggle play/pause
                if (video.paused) {
                    video.play();
                } else {
                    video.pause();
                }
            }
            updateMuteButtons();
        });
    });
});

function syncMuteStateAcrossVideos() {
    const videos = document.querySelectorAll(".reel-item video");
    videos.forEach(video => {
        video.muted = globalMuted;
    });
    updateMuteButtons();
}

function updateMuteButtons() {
    const muteButtons = document.querySelectorAll(".mute-btn i");
    muteButtons.forEach(icon => {
        if (globalMuted) {
            icon.className = "bi bi-volume-mute-fill";
        } else {
            icon.className = "bi bi-volume-up-fill";
        }
    });
}

window.toggleMuteGlobal = function () {
    globalMuted = !globalMuted;
    syncMuteStateAcrossVideos();
};

window.toggleLike = function (id, url) {
    if (typeof $ === 'undefined') {
        console.error("jQuery is not loaded!");
        return;
    }

    $.post(url, { reelId: id }, function (data) {
        const countElem = document.getElementById('like-count-' + id);
        const btnElem = document.getElementById('like-btn-' + id);
        const iconElem = btnElem ? btnElem.querySelector('i') : null;

        if (countElem) countElem.innerText = data.likeCount;
        if (btnElem) btnElem.classList.toggle('btn-danger-active', data.isLiked);
        if (iconElem) iconElem.className = data.isLiked ? 'bi bi-heart-fill' : 'bi bi-heart';
    }).fail(function () {
        console.error("Failed to update like for reel:", id);
    });
};

window.handleWatch = function (id, videoElement, url) {
    const endTime = Date.now();
    const start = videoElement.startTime || endTime;
    const duration = (endTime - start) / 1000;
    const percentage = videoElement.duration ? (videoElement.currentTime / videoElement.duration) * 100 : 0;

    recordWatch(id, url, Math.round(duration), Math.round(percentage));
    videoElement.startTime = Date.now();
};

window.updateProgress = function (id, video) {
    if (!video.duration) return;
    const fill = document.getElementById('progress-' + id);
    if (fill) fill.style.width = ((video.currentTime / video.duration) * 100) + '%';
};

function recordWatch(id, url, duration, percentage) {
    if (typeof $ === 'undefined') return;

    $.post(url, {
        reelId: id,
        watchDurationSec: duration,
        watchPercentage: percentage
    });
}
