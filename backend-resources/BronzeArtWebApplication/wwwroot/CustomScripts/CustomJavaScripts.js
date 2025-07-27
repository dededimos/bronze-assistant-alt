//Scroll to a particular element and Mark Border with animation
function scrollToElementWithAnimation(id, animationClassName, animationDurationSec) {
    var element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behaviour: 'smooth', block: 'end', inline: 'end' });
        element.style.animation = `${animationClassName} ${animationDurationSec}s ease`;

        // Remove the animation after a certain time
        setTimeout(() => {
            element.style.animation = '';
        }, animationDurationSec * 1000);
    }
}

//Scrolls to an Element
function scrollToElement(id) {
    var element = document.getElementById(id);
    if (element) {
        element.scrollIntoView({ behaviour: 'smooth', block: 'nearest', inline: 'start' });
    }
}

//Scrolls back to top
function scrollToTop() {
    document.documentElement.scrollTop = 0;
}
//Scrolls back to top a certain element
function scrollToTopElement(id) {
    var element = document.getElementById(id);
    if (element) {
        element.scrollTop = 0;
    }
}

// Save Generated File from Base64string
function saveAsFile(filename, bytesBase64) {
    var link = document.createElement('a');
    link.download = filename ?? '';
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); //Needed for Firefox
    link.click();
    document.body.removeChild(link);
}

function downloadExcelFile(bytesBase64, fileName) {
    var link = document.createElement('a');
    link.href = 'data:application/vnd.openxmlformats-officedocument.spreadsheetml.sheet;base64,' + bytesBase64;
    link.download = fileName;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
}


//Download File from Existing url
function triggerFileDownload(fileName, url) {
    const anchorElement = document.createElement('a');
    anchorElement.href = url;
    anchorElement.download = fileName ?? '';
    anchorElement.type = 'application/octet-stream';
    anchorElement.click();
    anchorElement.remove();
}

//Gets window Dimensions
function GetWindowDimensions() {
    return {
        width: window.innerWidth,
        height: window.innerHeight
    };
};

function printPage() {
    window.print();
    setTimeout(function () { window.history.back(); }, 1000);
     //Goes back Right After Printing (Waits 1 sec first so the page can be loaded properly before going back)
}

function printCurrentPage() {
    window.print();
}

function StartingLogoCheck(){
    if (window.location.origin === "https://lakiotis.bronzeapp.gr") {
        let whitelabelLogo = document.createElement('img');
        whitelabelLogo.src = '../Images/Logos/Whitelabel1.svg';
        whitelabelLogo.style = 'max-width:30vw';
        let brElement = document.createElement('br')
        document.getElementById('StartingLogoContainer').appendChild(whitelabelLogo);
        document.getElementById('StartingLogoContainer').appendChild(brElement);
    }
}

function getBaseUri() {
    return window.location.origin;
}

//Checks if clicked container contains currently active(focused)element if Not gives focus to given Id
function SetFocusToOnContainerClick(id,idContainer) {
    if (document.getElementById(idContainer).contains(document.activeElement)) {
        return
    }
    else {
        document.getElementById(id).focus();
    }
}


function LogLengths() {
    const logo = document.querySelectorAll("#BronzeArtLogo2Path");
    for (var i = 0; i < logo.length; i++) {
        console.log(`Letter ${i} is ${logo[i].getTotalLength()}`);
    }
    console.log(logo.length);

    const path123 = document.getElementById("BronzeArtLogo2Path");
    var length = path123.getTotalLength();
    console.log(length);
}


function focusElement(id) {
    var element = document.getElementById(id);
    if (element) {
        element.focus();
    }
}

function focusInputInsideParent(parentId) {
    var parent = document.getElementById(parentId);
    if (parent) {
        var inputInsideParent = parent.querySelector('input');
        if (inputInsideParent) {
            inputInsideParent.focus();
        }
    }
}

//Copies a table to clipboard by its Id
function copyTableToClipboard(tableId, columnIndicesToRemove = [], rowIndicesToRemove = []) {
    
    // Clone the table to avoid altering the original
    const table = document.getElementById(tableId);
    copyTableToClipboardByTableElement(table, columnIndicesToRemove, rowIndicesToRemove);
}
//Copies a table to clipboard
function copyTableToClipboardByTableElement(table, columnIndicesToRemove = [], rowIndicesToRemove = []) {

    const clonedTable = table.cloneNode(true);

    // Create a temporary container for the cloned table
    let container = document.createElement('div');
    container.appendChild(clonedTable);

    // Remove columns and rows if specified
    removeRows(clonedTable, ...rowIndicesToRemove);
    removeColumns(clonedTable, ...columnIndicesToRemove);

    if (clonedTable) {
        navigator.clipboard.write([
            new ClipboardItem({
                "text/html": new Blob([container.outerHTML], { type: "text/html" })
            })
        ]).then(() => {
            console.log('Table HTML copied to clipboard');
        }).catch(err => {
            console.error('Error in copying table HTML: ', err);
        });
    }
}
//Copies the first table it finds residing inside another element
function copyTableInsideElement(elementId, columnIndicesToRemove = [], rowIndicesToRemove = []) {
    var table = findTableInsideElement(elementId);
    if (table) {
        copyTableToClipboardByTableElement(table, columnIndicesToRemove, rowIndicesToRemove);
    }
}

function removeColumns(table, ...columnIndices) {
    if (table) {
        const sortedIndices = columnIndices.sort((a, b) => b - a);
        Array.from(table.rows).forEach(row => {
            sortedIndices.forEach(index => {
                if (row.cells.length > index) {
                    row.deleteCell(index);
                }
            });
        });
    }
}

function removeRows(table, ...rowIndices) {
    if (table) {
        const sortedIndices = rowIndices.sort((a, b) => b - a);
        sortedIndices.forEach(index => {
            if (table.rows.length > index) {
                table.deleteRow(index);
            }
        });
    }
}

//Finds the first table inside a certain element
function findTableInsideElement(elementId) {
    var element = document.getElementById(elementId);
    if (element) {
        var table = element.querySelector("table");
        return table;
    }
    return null;
}



//function preLoadAllImages() {
//    var img1 = new Image(); img1.src = "../Images/MirrorsImages/AllRectangularMirrors.jpg,
//}