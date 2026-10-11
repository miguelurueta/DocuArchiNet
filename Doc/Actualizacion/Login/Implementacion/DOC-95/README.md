# DOC-95 — Orquestación Login 2FA y frontera ASMX

## Resultado

DOC-95 conecta el segundo factor ya modelado en DOC-91 a DOC-94 con el login legacy. Cuando el módulo tiene 2FA activo, el postback de `gestor.aspx` crea y envía un OTP, conserva el contexto únicamente en Session y detiene la autenticación. La frontera `webservice/WebServiceLoginSegundoFactor.asmx` permite consultar estado, verificar, reenviar y cancelar. La cookie de Forms Authentication se emite únicamente después de ganar la adquisición del challenge, finalizar el login legacy y confirmar `Complete`.

Con 2FA desactivado, los cuatro tipos de módulo mantienen el recorrido legacy. No se agregan ni modifican tablas, columnas, índices, procedimientos, datos productivos, `ClassCorreo` o recuperación de contraseña.

## Alcance verificado

- Repositorio revisado: `DocuArchiNet`.
- Módulos: entrada de login común y los tipos `DOCUARCHI CONTENEDOR`, `GESTOR DOCUMENTAL`, `RADICACION` y `WORKFLOW` resueltos por DOC-94.
- Capas: contratos/modelos/DTO, servicio de aplicación, adaptadores WebForms, ASMX, composición, pruebas locales, E2E registrada y documentación.
- Reutilización: persistencia DOC-92 sobre `ra_auth_second_factor_challenge`, SMTP DOC-93 sobre la configuración activa `ESTADO_ENVIO=1`, principal/módulo DOC-94 y finalizador `ClassGestorSesion.FinalizeLogin`.

La interfaz visual para capturar el OTP está fuera de DOC-95. La frontera ASMX sí está implementada y puede probarse antes de incorporar esa UI.

## Documentos

- `01-ARQUITECTURA-Y-DIAGRAMAS.md`: límites, decisiones y trazabilidad de diagramas.
- `02-CASOS-DE-USO.md`: cinco casos implementados y la integración con preautenticación.
- `03-INVENTARIO-TECNICO.md`: endpoints, servicios, puertos, adaptadores, modelos y DTO.
- `04-VALIDACION-Y-PENDIENTES.md`: comandos reales, resultados, riesgos, rollback y asuntos abiertos.
- `diagram-contract.json`: inventario obligatorio de documentos, diagramas y símbolos estructurales.

## Convención visual

- `CODE:` identifica una clase, interfaz o método que debe resolverse estructuralmente en el repositorio.
- `EXT:` identifica un actor o sistema externo y es excluido explícitamente de resolución.
- `CONCEPT:` identifica una condición, estado lógico o decisión y es excluido explícitamente de resolución.

La prueba documental valida existencia, sintaxis Mermaid y correspondencia estructural de tipos, relaciones, propiedades y firmas mediante Roslyn. No demuestra por sí sola fidelidad conductual completa ni cobertura de todos los casos; esa evidencia se complementa con pruebas conductuales y, cuando exista autorización, E2E real.
