function hideAssociatedFloorsSection_OnCreate(executionContext) {
    var formContext = executionContext.getFormContext();

    // Form types:
    // 0 - Undefined
    // 1 - Create
    // 2 - Update
    // 3 - Read Only
    // 4 - Disabled
    // 6 - Bulk Edit
    var formType = formContext.ui.getFormType();

    if (formType === 1) { 
        var section = formContext.ui.tabs.get("genralTab").sections.get("section_associatedFloors");
        if (section) {
            section.setVisible(false);
        }
        else if(formType === 2) {
            section.setVisible(true);
        }
    }
}
