window.setupMedia = async (videoElementId) => {
    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true, audio: true });

        const videoElement = document.getElementById(videoElementId);
        
        if (videoElement) {
            videoElement.srcObject = stream;
        }
    } catch (error) {
        console.error('Error accessing camera and microphone:', error);
    }
};

window.stopMedia = (videoElementId) => {
    const videoElement = document.getElementById(videoElementId);
    if (videoElement && videoElement.srcObject) {
        const tracks = videoElement.srcObject.getTracks();
        tracks.forEach(track => track.stop());
        videoElement.srcObject = null;
    }
};