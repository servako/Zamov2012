function fadeScreenOut() {
    $(".shader").css("display", "block");
}

function fadeScreenIn() {
    $(".shader").css("display", "none");
}


function tableChanged(dictionary, field) {
    var fieldSegments = field.name.split("_");

    var fieldName = fieldSegments[0];
    var id = fieldSegments[1];
    if (dictionary[id.toString()] == null) {
        dictionary[id.toString()] = {};
    }
    if (field.type == "checkbox") {
        dictionary[id.toString()][fieldName] = field.checked;
    }
    else {
        dictionary[id.toString()][fieldName] = field.value;
    }
}

function collectChanges(dictionary, hiddenId) {
    var hidden = $get(hiddenId);
    hidden.value = Sys.Serialization.JavaScriptSerializer.serialize(dictionary);
    return true;
}

function failureCallback(error) {
}

var lastCorrectQuantityValues = {};

function validateQuantity(el) {
    var rExp = /^\d{1,4}$/;
    if (!rExp.test(el.value)) {
        if (el.value.length > 0)
            el.value = (lastCorrectQuantityValues[el.id]) ? lastCorrectQuantityValues[el.id] : "";
    }
    else {
        lastCorrectQuantityValues[el.id] = el.value;
    }
}