function OnLoad() {
    MakeObserv();
    ApplyBinds();
    GetStores();
    ButtonClick();
}

function MakeObserv() {
    var that = this;
    that.storetable = ko.observable();
    that.selectedstore = ko.observable();
    that.storemodel = ko.observable();
    that.btnlabel = ko.observable();
    that.modaltitle = ko.observable();
    that.storeid = ko.observable();
    that.storename = ko.observable();
    that.storeaddress = ko.observable();
    that.cansave = ko.observable();
}

function GetStores() {
    var that = this;
    $.getJSON("/Store/GetStores", function (result) {
        that.storetable(JSON.parse(result.Stores));
    })
        .error(function () {
            alert("Unable to load Stores page. Contact an administrator.");
        });
}

function ApplyBinds() {
    ko.applyBindings();
}

function SaveChanges() {
    var that = this;
    var id = selectedstore().storeid();
    var name = selectedstore().storename();
    var address = selectedstore().storeaddress();
    $.ajax({
        url: "/Store/Edit",
        data: {
            Id: id,
            StoreName: name,
            StoreAddress: address
        },
        type: 'POST',
        datatype: JSON
    })
            .success(function (result) {
                alert(result.Message);
                location.reload();
            })
            .error(function(xhr, status){
            alert("Unable to save changes. Contact the administrator");
        })
}

function ButtonClick() {
    var that = this;
    that.mouseClickCreate = function () {
        var that = this;
        var id = 0;
        $.ajax({
            url: "/Store/Edit",
            data: { Id: id },
            type: 'GET',
            datatype: 'json'
        })
            .success(function (result) {
            that.selectedstore(new StoreViewModel(result));
            $("#updatemodal").modal("show");
            that.btnlabel("Create");
            that.modaltitle("Create Record");
        })
        .error(function (xhr, status) {
            alert("Unable to load Create View. Contact the administrator.");
        })
    }

    that.mouseClickEdit = function (d) {
        var that = this;
        $.ajax({
            url: "/Store/Edit",
            data: { Id: d.Id },
            type: "GET",
            datatype: "json"
        })
            .success(function (result) {
                that.selectedstore(new StoreViewModel(result)); 
                $("#updatemodal").modal("show");
                that.btnlabel("Save");
                that.modaltitle("Edit Record");
            })
    }

    that.mouseClickDelete = function (d) {
        var that = this;
        if (confirm("Are you sure?")) {
            $.ajax({
                url: "/Store/Delete",
                data: { Id: d.Id },
                type: "POST",
                datatype: 'json'
            })
                .success(function (result) {
                    alert(result.Message);
                    location.reload();
                })
                .error(function (xhr, status) {
                    alert("Unable to delete record. Contact the administrator.");
                })
        }
    };
}

function StoreViewModel(data) {
    var that = this;
    that.storeid = ko.observable(data.Id);
    that.storename = ko.observable(data.Name).extend({ required: { params: true, message: 'Enter a Store' } });
    that.storeaddress = ko.observable(data.Address).extend({ required: { params: true, message: 'Enter an Address' } });
    that.validationModel = ko.validation.group(that);
    that.cansave = ko.computed(function () {
        that.validationModel.showAllMessages();
        return that.validationModel().length == 0;
    });
}