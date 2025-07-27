function GetWidthOfElement(elementId) {
    let element = document.getElementById(elementId);
    let elementWidth = element.clientWidth;
    return elementWidth;
}

function GetHeightOfElement(elementId) {
    let element = document.getElementById(elementId);
    let elementHeight = element.clientHeight;
    return elementHeight;
}
