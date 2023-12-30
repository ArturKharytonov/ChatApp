export function setLocalStream(stream) {
    const localVideo = document.getElementById('localVideo');
    localVideo.srcObject = stream;
}
export function setRemoteStream(stream) {
    const remoteVideo = document.getElementById('remoteVideo');
    remoteVideo.srcObject = stream;
}

export function setRemoteStreamToNull() {
    const remoteVideo = document.getElementById('remoteVideo');
    if (remoteVideo.srcObject) {
        const tracks = remoteVideo.srcObject.getTracks();
        tracks.forEach(track => track.stop());

        remoteVideo.srcObject = null;
    }
}

export function stopCameraAndMic(stream) {
    if (!stream) {
        console.error('Stream is null or undefined.');
        return;
    }

    stream.getTracks().forEach((track) => {
        if (track && track.readyState == 'live') {
            track.stop();
        }
    });
}