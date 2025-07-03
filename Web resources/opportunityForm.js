function confirmDetails_onChange(executionContext) {
    var formContext = executionContext.getFormContext();

    var confirmDetails = formContext.getAttribute("giulia_confirmdetails").getValue();
    if (confirmDetails === false) {
        formContext.ui.setFormNotification("'Confirm details' field should be equal to 'Yes'", "INFO", "_myUniqueId");
    } else {
        formContext.ui.clearFormNotification("_myUniqueId");
    }
}


function lockFieldsBPF(executionContext) {
    var formContext = executionContext.getFormContext();    
    
    formContext.getControl("header_process_giulia_rentcost").setDisabled(true);
    formContext.getControl("header_process_giulia_offersent").setDisabled(true);
    formContext.getControl("header_process_giulia_offerapproved").setDisabled(true);
    formContext.getControl("header_process_giulia_contractpreparationstatus").setDisabled(true);
}

function setDefaultCurrency(executionContext) {
    var formContext = executionContext.getFormContext();

    if (formContext.ui.getFormType() === 1) { // Only on Create
        formContext.getAttribute("transactioncurrencyid").setValue([{
            id: "1e7385cb-1245-f011-877a-000d3a47a722", // Euro GUID
            name: "Euro",
            entityType: "transactioncurrency"
        }]);
    }
}


function filterFloorLookup(executionContext) {
    var formContext = executionContext.getFormContext();
    
    //Get the floorControl from the BPF header
    var floorControl = formContext.getControl("header_process_giulia_floor"); 
    if (!floorControl) {
        console.warn("BPF Floor control not found");
        return;
    }

    floorControl.addPreSearch(function () {
        //Obtain the needen fields from the form
        const mallRef = formContext.getAttribute("giulia_mall")?.getValue(); 
        const offeredSpace = formContext.getAttribute("giulia_offeredspace")?.getValue();

        formContext.ui.clearFormNotification("_myUniqueId");

        if (!mallRef || offeredSpace == null) {
            formContext.ui.setFormNotification("Please fill in 'Mall' and 'Offered space' before selecting a floor.", "INFO", "_myUniqueId");
            return;
        }

        const mallId = mallRef[0].id.replace(/[{}]/g, "");
        const safeOfferedSpace = parseFloat(offeredSpace);

        if (isNaN(safeOfferedSpace)) {
            formContext.ui.setFormNotification("'Offered space' must be a valid number.", "ERROR", "_myUniqueId");
            return;
        }

        const fetchXml = `<filter type="and">
            <condition attribute="giulia_mall" operator="eq" value="${mallId}" />
            <condition attribute="giulia_availablespace" operator="gt" value="${safeOfferedSpace}" />
            <condition attribute="giulia_totalspace_gt_occupiedspace" operator="eq" value="1" />
        </filter>`;
        floorControl.addCustomFilter(fetchXml, "giulia_floor");
    });
}

function handleReadOnlyFieldsOnContractStatus(executionContext) {
    var formContext = executionContext.getFormContext();
    
    var statusReasonAttr = formContext.getAttribute("giulia_statusreason");
    if(!statusReasonAttr)
        return;
        
    var statusReasonValue = statusReasonAttr.getValue();
    var shouldBeVissible = (statusReasonValue === 343530003);

    formContext.getControl("giulia_offeredspace").setDisabled(shouldBeVissible);
    formContext.getControl("giulia_priceperm2").setDisabled(shouldBeVissible);
    
}

function offeredSpaceCheckIfAvailable(executionContext) {
    formContext = executionContext.getFormContext();
    
    var offeredSpace = formContext.getAttribute("giulia_offeredspace").getValue();
    var floorRef = formContext.getAttribute("giulia_floor").getValue();
    
    //check if floorRef is not null
    if(floorRef && floorRef.length > 0) {
        //get floor id
        var floorId = floorRef[0].id.replace(/[{}]/g, "");
        
        Xrm.WebApi.retrieveRecord("giulia_floor", floorId, "?$select=giulia_totalspace, giulia_occupiedspace")
            .then(function(result) {
                var totalSpace = result.giulia_totalspace;
                var occupiedSpace = result.giulia_occupiedspace;
                
                console.log("Total space:", totalSpace);
                console.log("Occupied space:", occupiedSpace);
                console.log("Available space:", floorAvailableSpace);
                console.log("Offered space:", offeredSpace);
                
                //save the available space
                var floorAvailableSpace = totalSpace - occupiedSpace;
                
                //check if the availableSpace is greater than offeredSpace
                if(floorAvailableSpace < offeredSpace) {
                    Xrm.Navigation.openAlertDialog({text: "Offered space is not valid! The only available space is " + floorAvailableSpace.toString() + "m2."});
                    return;
                }         
            })
            .catch(function (error) {
                console.error("Error retrieving floor record:", error.message);
            });
    }
}

function opportunitySubmitForApproval(primaryControl) {
    var formContext = primaryControl;
    var opportunityId = formContext.data.entity.getId().replace(/[{}]/g, "");
    
    var requestBody = {
        OpportunityId: opportunityId
    };
    var actionName = "Microsoft.Dynamics.CRM.giulia_SubmitOpportunityForApprovalAction";
    var entitySetName = "giulia_opportunities";
    //var url = "https://endava365internship.crm4.dynamics.com/" 
     //         +"/api/data/v9.2/" + entitySetName + "(" + opportunityId + ")/" + actionName;
              
              var url = Xrm.Utility.getGlobalContext().getClientUrl()
        + "/api/data/v9.2/" + entitySetName + "(" + opportunityId + ")/" + actionName;


    
    var req = new XMLHttpRequest();
    req.open("POST", url, true);
    req.setRequestHeader("OData-MaxVersion", "4.0");
    req.setRequestHeader("OData-Version", "4.0");
    req.setRequestHeader("Accept", "application/json");
    req.setRequestHeader("Content-Type", "application/json; charset=utf-8");
    req.setRequestHeader("Prefer", "return=representation");
    
    req.onreadystatechange = function() {
        //4 - DONE
        if(req.readyState === 4) {
            if (req.status !== 200 && req.status !== 204){
                var error= JSON.parse(req.responseText).error;
                Xrm.Navigation.openAlertDialog({text: "Error: " + error.message});
            }
            else {
                Xrm.Navigation.openAlertDialog({ text: "Opportunity submitted for approval successfully." }).then(function () {
                    refresh(formContext);
                });
            }
        }
    };
    
    req.send(JSON.stringify(requestBody));
}

function refresh(formContext) {
  
    // Save the form if it's dirty (has unsaved changes)
    if (formContext.data.entity.getIsDirty()) {
        formContext.data.save().then(function () {
            formContext.data.refresh(true).then(function () {
                formContext.ui.refreshRibbon();
            }, function (error) {
                console.error("Data refresh failed:", error.message);
            });
        }, function (error) {
            console.error("Save failed:", error.message);
        });
    } else {
        // No unsaved changes — just refresh from server
        formContext.data.refresh(true).then(function () {
            formContext.ui.refreshRibbon();
        }, function (error) {
            console.error("Data refresh failed:", error.message);
        });
    }
}