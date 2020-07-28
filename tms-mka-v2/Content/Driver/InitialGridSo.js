var dsMasterDriver;
var gridMasterDriver;
var Caller;

function GetHistoryJalan(Id) {
    var dsDetailHis = new kendo.data.DataSource({
        transport: {
            read: {
                url: '/Driver/GetHistoryJalan?id=' + Id,
                dataType: "json"
            },
        },
        pageSize: 10,
        pageable: true,
        sortable: true,
    });

    $("#GridHistoryJalanDriver").kendoGrid({
        dataSource: dsDetailHis,
        filterable: kendoGridFilterable,
        sortable: true,
        reorderable: true,
        resizable: true,
        pageable: true,
        columns: [
        { field: "shipmentId", title: "shipment ID" },
        { field: "noSo", title: "No SO" },
        { field: "tanggalMuat", title: "Tgl Muat", template: "#= tanggalMuat != null ? kendo.toString(kendo.parseDate(tanggalMuat, 'yyyy-MM-dd'), 'dd/MM/yyyy') : ''#" },
        { field: "jenisOrder", title: "Jenis Order" },
        { field: "customer", title: "Customer" },
        { field: "rute", title: "Rute" },
        ],
    });
}

$(document).ready(function () {
    dsMasterDriver = new kendo.data.DataSource({
        transport: {
            read: {
                url: '/Driver/BindingDetailSo?idSo=' + IdSo,
                dataType: "json"
            },
        },
        schema: {
            total: "total",
            data: "data",
            model: {
                fields: {
                    "Id": { type: "number" },
                    "Status": { type: "string" },
                    "StatusSo" : {type: "string"},
                    "KodeDriver": { type: "string" },
                    "NamaDriver": { type: "string" },
                    "NamaPangilan": { type: "string" },
                    "DokumenPending": { type: "string" },
                    "TglBerlakuSim": { type: "string" },
                    "Training": { type: "string" },
                }
            }
        },
        pageSize: 5,
        pageable: true,
        sortable: true,
    });

    gridMasterDriver = $("#GridMasterDriver").kendoGrid({
        dataSource: dsMasterDriver,
        filterable: kendoGridFilterable,
        sortable: true,
        reorderable: true,
        resizable: true,
        pageable: true,
        groupable: true,
        columns: [
            {
                command: [
                    {
                        name: "select",
                        text: "Select",
                        click: SelectDriver,
                        imageClass: "glyphicon glyphicon-ok",
                        template: '<a class="k-button-icon #=className#" #=attr# href="\\#"><span class="#=iconClass# #=imageClass#"></span></a>'
                    }
                ],
                width: "50px"
            },
            { field: "StatusSo", title: "Status Order" },
            { field: "KodeDriver", title: "Kode Driver" },
            { field: "NamaDriver", title: "Nama Driver", template: "<a href='\\#' data-toggle='modal' data-target='\\#ModalHistoryJalan' onclick='GetHistoryJalan(#:Id#)'> #= NamaDriver # </a>" },
            { field: "NamaPangilan", title: "Nama Panggilan" },
            { field: "DokumenPending", title: "Dokumen Pending" },
            { field: "TglBerlakuSim", title: "Sim Expired", template: "#= TglBerlakuSim != null ? kendo.toString(kendo.parseDate(TglBerlakuSim, 'yyyy-MM-dd'), 'dd/MM/yyyy') : ''#" },
            { field: "Training", title: "Training" }
        ],
    }).data("kendoGrid");

    var dsHB = new kendo.data.DataSource({
        transport: {
            read: {
                url: '/Driver/BindingSettlementBatal?id=' + $('#IdDriver1').val(),
                dataType: "json"
            },
        },
        schema: {
            total: "total",
            data: "data",
            model: {
                fields: {
                    "Id": { type: "number" },
                    "JenisOrder": { type: "string" },
                    "NoDn": { type: "string" },
                    "NoSo": { type: "string" },
                    "Customer": { type: "string" },
                    "VehicleNo": { type: "string" },
                    "Driver": { type: "string" },
                    "JenisBatal": { type: "string" },
                    "Tanggal": { type: "date" },
                    "IsProses": { type: "bool" },
                }
            }
        },
        pageSize: 10,
        pageable: true,
        //serverFiltering: true,
        //serverPaging: true,
        //serverSorting: true,
        sortable: true,
    });

    $("#GridHistoryBatal").kendoGrid({
                dataSource: dsHB,
                filterable: kendoGridFilterable,
                sortable: true,
                reorderable: true,
                resizable: true,
                pageable: true,
                groupable: true,
                //height: "615",
                columns: [
                    {
                        field: "JenisOrder",
                        title: "Jenis Order",
                    },
                    {
                        field: "NoDn",
                        title: "No.DN",
                    },
                    {
                        field: "NoSo",
                        title: "No.SO",
                    },
                    {
                        field: "Customer",
                        title: "Customer",
                    },
                    {
                        field: "VehicleNo",
                        title: "No.Polisi",
                    },
                    {
                        field: "Driver",
                        title: "Driver",
                    },
                    {
                        field: "Tanggal",
                        title: "Tanggal Batal",
                        template: "#= Tanggal != null ? kendo.toString(kendo.parseDate(Tanggal, 'yyyy-MM-dd'), 'dd/MM/yyyy') : ''#",
                    },
                    {
                        field: "JenisBatal",
                        title: "Jenis Batal",
                    },
                ],
            }).data("kendoGrid");

})