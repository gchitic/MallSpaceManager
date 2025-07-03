function hasLegalRole() {
    var userRoles = Xrm.Utility.getGlobalContext().userSettings.securityRoles;
    var legalRole = "8107ab28-8f4a-f011-877a-7c1e525e6b53";
    for(var i=0; i< userRoles.length; i++) {
        if(userRoles[i] == legalRole)
            return true;
    }
    return false;
}

function openContractForm(primaryControl) {
    debugger;
    var formContext = primaryControl;
    var opportunityId = formContext.data.entity.getId().replace(/[{}]/g, "");
    
    var entityFormOptions = {};
    entityFormOptions["entityName"] = "giulia_contract";

    var account = formContext.getAttribute("giulia_account")?.getValue();
    var mall = formContext.getAttribute("giulia_mall")?.getValue();
    var rentCost = formContext.getAttribute("giulia_rentcost")?.getValue();
    var topic = formContext.getAttribute("giulia_topic")?.getValue(); 

    var formParameters = {};

    // Account lookup
    if (account && account[0]) {
        formParameters["giulia_account"] = account[0].id.replace(/[{}]/g, "");
        formParameters["giulia_accountname"] = account[0].name;
        formParameters["giulia_accounttype"] = account[0].entityType;
    }

    // Mall lookup
    if (mall && mall[0]) {
        formParameters["giulia_mall"] = mall[0].id.replace(/[{}]/g, "");
        formParameters["giulia_mallname"] = mall[0].name;
        formParameters["giulia_malltype"] = mall[0].entityType;
    }

    // Rent cost
    if (rentCost !== null && rentCost !== undefined) {
        formParameters["giulia_rentcost"] = rentCost;
    }

    // Opportunity lookup
    formParameters["giulia_opportunity"] = opportunityId;
    formParameters["giulia_opportunityname"] = topic;
    formParameters["giulia_opportunitytype"] = "giulia_opportunity";

    // Open the new form
    Xrm.Navigation.openForm(entityFormOptions, formParameters).then(
        function (success) {
            console.log("Contract form opened successfully:", success);
        },
        function (error) {
            console.error("Failed to open Contract form:", error);
        }
    );
}
