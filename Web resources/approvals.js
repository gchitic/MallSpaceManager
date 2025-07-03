function hasApprovalRole()
{
	var userRoles = Xrm.Utility.getGlobalContext().userSettings.securityRoles;
	var approvalRoles = [
        "f1464e4c-2748-f011-877a-000d3a47a722", // ApprovalTeam1
        "4911e6ae-2848-f011-877a-7c1e525e6b53", // ApprovalTeam2
        "b2c9d7da-2848-f011-877a-7c1e525e6b53" // ApprovalTeam3
    ];
	for (var i = 0; i < userRoles.length; i++)
	{
		if (approvalRoles.includes(userRoles[i]))
		{
			return true;
		}
	}
    console.error("hasApprovalRole failed:");
	return false;
}

function opportunityApprove(primaryControl)
{
	debugger;
	var formContext = primaryControl;
	var opportunityId = formContext.data.entity.getId().replace(/[{}]/g, "");
	var requestBody = {
		//OpportunityId: opportunityId
	};
	var actionName = "Microsoft.Dynamics.CRM.giulia_ApproveOpportunityAction";
	var entitySetName = "giulia_opportunities";
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
                Xrm.Navigation.openAlertDialog({ text: "Opportunity approved successfully." });
                
                formContext.data.save().then(function() {
                    // Move to the next stage
                    formContext.data.process.moveNext(function(result) {
                        if (result === "success") {
                            console.log("BPF advanced without needing a full refresh");
                        } else {
                            console.error("Could not advance BPF stage:", result);
                        }
                    });
                });
            }

        }
    };
	req.send(JSON.stringify(requestBody));

}

async function evaluateUserCanApprove(executionContext) {
    const formContext = executionContext.getFormContext();
    const opportunityId = formContext.data.entity.getId().replace(/[{}]/g, "");
    const stage = formContext.getAttribute("giulia_statusreason").getValue();
    const userRoles = Xrm.Utility.getGlobalContext().userSettings.securityRoles;

    const approvalRoles = [
        { Id: "f1464e4c-2748-f011-877a-000d3a47a722", Name: "Approval Team 1" },
        { Id: "4911e6ae-2848-f011-877a-7c1e525e6b53", Name: "Approval Team 2" },
        { Id: "b2c9d7da-2848-f011-877a-7c1e525e6b53", Name: "Approval Team 3" }
    ];

    const matchedRoles = userRoles
        .map(roleId => approvalRoles.find(r => r.Id.toLowerCase() === roleId.toLowerCase()))
        .filter(r => r !== undefined);

    if (matchedRoles.length === 0) {
        formContext.getAttribute("giulia_usercanapprove").setValue(false);
        formContext.getAttribute("giulia_usercanapprove").setSubmitMode("always");
        formContext.ui.refreshRibbon();
        return;
    }

    const approvalTeamFilter = matchedRoles.map(r =>
        `giulia_approvalteam eq '${r.Name.replace(/'/g, "''")}'`
    ).join(" or ");

    const fetchFilter = `?$filter=_giulia_opportunity_value eq ${opportunityId} and (${approvalTeamFilter})`;

    try {
        const result = await Xrm.WebApi.retrieveMultipleRecords("giulia_approvals", fetchFilter);
        const userApprovals = result.entities;
        const alreadyApprovedCount = userApprovals.filter(r => r["giulia_status"] === 343530000).length;

        const shouldShowApproveButton = (
            stage === 343530002 && alreadyApprovedCount < matchedRoles.length
        );

        formContext.getAttribute("giulia_usercanapprove").setValue(shouldShowApproveButton);
        formContext.getAttribute("giulia_usercanapprove").setSubmitMode("always");
        formContext.ui.refreshRibbon();

    } catch (error) {
        formContext.getAttribute("giulia_usercanapprove").setValue(false);
        formContext.ui.refreshRibbon();
    }
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
