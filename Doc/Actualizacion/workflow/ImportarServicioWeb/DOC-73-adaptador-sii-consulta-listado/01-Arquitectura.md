# Arquitectura

La ruta ejecutada es `UI DOC-72 → Registry → SiiAdapter → API DOC-72 → ASMX moderno`. El adaptador solo se registra para `INTEGRACIONSII`. `SiiContractMapper` proyecta DTOs normalizados y `SiiList` mantiene filtros, página y selección en memoria.

No se modifican ni invocan directamente `WebService_integracion_sii.asmx.vb`, `Class_consultarInformacionSello`, almacenamiento, caché o auditoría. El gate y la autorización permanecen en la integración/endpoint existentes.

