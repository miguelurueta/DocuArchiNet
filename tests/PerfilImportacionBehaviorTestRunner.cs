using System;
public static class PerfilImportacionBehaviorTestRunner {
    public static int Main() {
        try {
            PerfilImportacionReglasBehaviorTests.Run();
            PerfilImportacionServiceBehaviorTests.Run();
            PerfilImportacionRequestGuardBehaviorTests.Run();
            PerfilImportacionCatalogBehaviorTests.Run();
            PerfilImportacionMigrationBehaviorTests.Run();
            ProveedorIndicesBehaviorTests.Run();
            ProveedorIndicesReglasBehaviorTests.Run();
            PoliticaFuentesProveedorBehaviorTests.Run();
            AdministracionProveedorBehaviorTests.Run();
            FuenteImportacionBehaviorTests.Run();
            Console.WriteLine("DOC-110 behavior tests: passed");
            return 0;
        } catch (Exception error) {
            Console.Error.WriteLine(error.Message);
            return 1;
        }
    }
}
