var dsStat;

$(document).ready(function () {
    call_piutang()
    dsStat = new kendo.data.DataSource({
        transport: {
            read: {
                url: '/Driver/BindingStatHistory?id=' + $("#Id").val(),
                dataType: "json"
            },
        },
        schema: {
            model: {
                fields: {
                    Tanggal: { type: "string" },
                    Status: { type: "string" },
                    keterangan: { type: "string" },
                }
            }
        },
    });
});
    function call_piutang(piutang_type){
        dsPiutang = new kendo.data.DataSource({
            transport: {
                read: {
                    url: '/Driver/BindingPiutangHistory?id=' + $("#Id").val() + '&piutang_type=' + piutang_type ,
                    dataType: "json"
                },
            },
            schema: {
                model: {
                    fields: {
                        Tanggal: { type: "string" },
                        Keterangan: { type: "string" },
                        Jumlah: { type: "number" },
                        Saldo: { type: "number" },
                        Id: { type: "number" },
                    }
                }
            },
        });

        $("#gridPiutang").kendoGrid({
            dataSource: dsPiutang,
            columns: [
                {
                    field: "Tanggal",
                    title: "Tanggal",
                    template: "#= kendo.toString(kendo.parseDate(Tanggal, 'yyyy-MM-dd'), 'MM/dd/yyyy') #"
                },
                {
                    field: "Keterangan",
                    title: "Keterangan",
                    template: kendo.template($("#jenisOrder-template").html())
                },
                {
                    field: "Jumlah",
                    template: 'Rp #: kendo.format("{0:n}", Jumlah)#',
                    attributes: { style: "text-align:right;" },
                    title: "Jumlah"
                },
                {
                    field: "Saldo",
                    template: 'Rp #: kendo.format("{0:n}", Saldo)#',
                    attributes: { style: "text-align:right;" },
                    title: "Saldo"
                }
            ],
        }).data("kendoGrid");
    }