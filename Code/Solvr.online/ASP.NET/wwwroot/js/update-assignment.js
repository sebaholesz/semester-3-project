const deleteAssignment = (assignmentId) => {
    let xhr = new XMLHttpRequest();
        xhr.open('DELETE', "/assignment/delete-assignment/" + assignmentId);
        xhr.send();
        xhr.onload = () => {
    document.body.innerHTML = xhr.response;
        };
    }
