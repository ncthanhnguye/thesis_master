function TranRegister() {
    $('[id$=GridView1] tr').click(function () {
        $('[id$=hdftext]').val($(this).find('td:eq(1)').text());
        $('[id$=btnResearch]').click();
    });
}