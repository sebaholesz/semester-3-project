const deleteAssignment = (assignmentId) => {
    let xhr = new XMLHttpRequest();
        xhr.open('PUT', "/assignment/delete-assignment/" + assignmentId);
        xhr.send();
        xhr.onload = () => {
    document.body.innerHTML = xhr.response;
        };
    }
