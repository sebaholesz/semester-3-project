const collection = JSON.parse('@Html.Raw(Newtonsoft.Json.JsonConvert.SerializeObject(ViewBag.Solutions))');
let currentIndex = 0;

    const displaySolution = (index) => {
    let solution = {...collection[index]};
        console.log(solution)
        $("#currentSolutionId").text(solution.SolutionId);
        $("#currentSolutionUserId").text(solution.UserId);
        $("#currentSolutionTimestamp").text(solution.Timestamp);
        $("#currentSolutionDescription").text(solution.Description);
    }

    if (collection.length > 0) {
    displaySolution(0);
    }

    const minusCurrentIndex = () => {
    console.log(currentIndex);
        if (currentIndex - 1 >= 0) {
    currentIndex -= 1;
            $("#plusCurrentIndexButton").attr("disabled", false);
        }
        if (currentIndex === 0) {
    $("#minusCurrentIndexButton").attr("disabled", true);
        }
        displaySolution(currentIndex);
        console.log(currentIndex);
    };

    const plusCurrentIndex = () => {
    console.log(currentIndex);
        if (currentIndex + 1 < collection.length) {
    currentIndex += 1;
            $("#minusCurrentIndexButton").attr("disabled", false);
        }
        if (currentIndex + 1 === collection.length) {
    $("#plusCurrentIndexButton").attr("disabled", true);
        }
        displaySolution(currentIndex);
        console.log(currentIndex);
    };

    const chooseSolution = () => {
        if (confirm('Do you really want to choose this solution?')) {

            const solutionId = collection[currentIndex].SolutionId;
            const xhrChooseSolutionBody = solutionId;
            const xhrGetSolution = new XMLHttpRequest();
            const xhrChooseSolution = new XMLHttpRequest();

            xhrChooseSolution.open('PUT', "/solution/choose-solution");
            xhrChooseSolution.setRequestHeader("Content-Type", "application/json;charset=UTF-8");
            xhrChooseSolution.send(JSON.stringify(xhrChooseSolutionBody));
            xhrChooseSolution.onload = () => {
                document.body.innerHTML = xhrChooseSolution.response
            };
        }
    }

    const rejectSolution = () => {
    console.log(currentIndex);
        if (collection.length - 1 > 0) {
    collection.splice(currentIndex, 1);
            $("#minusCurrentIndexButton").attr("disabled", true);
            $("#plusCurrentIndexButton").attr("disabled", false);
            currentIndex = 0;
        }

        if (collection.length === 1) {
    $("#minusCurrentIndexButton").attr("disabled", true);
            $("#plusCurrentIndexButton").attr("disabled", true);
        }
        displaySolution(currentIndex);
        console.log(currentIndex);
    }
