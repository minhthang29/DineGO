function handleCredentialResponse(response) {
    const idToken = response.credential;

    fetch('/Auth/LoginWithGoogleToken', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({ idToken })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                window.location.href = '/';
            } else {
                alert('Login failed: ' + data.message);
            }
        });
}