# Validación automática, resultados y pendientes

## Prueba de documentación

La validación se divide en dos procesos:

1. `node --test tests/doc91-technical-documentation.test.cjs`
   - exige el inventario explícito de cuatro diagramas y cinco documentos;
   - procesa cada `.mmd` con `mermaid.parse`, usando el parser oficial y un DOM aislado de `jsdom`;
   - verifica comentarios de fuentes y referencias, archivos existentes y convención `CODE/EXT/CONCEPT`;
   - comprueba que ninguna referencia `CODE:` quede sin contrato;
   - confirma que CI ejecute ambas validaciones.
2. `dotnet run --project ./tools/validation/Doc72SourceValidator/Doc72SourceValidator.csproj -- ./Doc/Actualizacion/Login/Implementacion/DOC-91/diagram-contract.json .`
   - usa Roslyn Visual Basic, no búsquedas textuales;
   - exige clases, interfaces y enums en su archivo exacto;
   - resuelve pertenencia, parámetros, `ByRef`, retorno y sobrecargas de métodos;
   - valida miembros de enums y propiedades/tipos de DTO.

La prueba falla indicando diagrama, archivo, símbolo y motivo ante ausencia, sintaxis inválida, fuente inexistente, referencia no declarada o firma inconsistente.

## Resultados

Ejecución local del 2026-10-09:

| Comando | Código | Resultado |
| --- | ---: | --- |
| `node --test tests/doc91-technical-documentation.test.cjs` | 0 | 4 pruebas aprobadas; Mermaid aceptó los cuatro diagramas requeridos. |
| `dotnet run --project .\tools\validation\Doc72SourceValidator\Doc72SourceValidator.csproj -- .\Doc\Actualizacion\Login\Implementacion\DOC-91\diagram-contract.json .` | 0 | PASS: 32 declaraciones, 2 enums, 34 firmas y 4 tipos con propiedades. |
| `node --test tests/login-second-factor-foundation.test.cjs` | 0 | 3 pruebas funcionales aprobadas en la ejecución previa de DOC-91. |

La CI instala dependencias mediante `npm.cmd --prefix tools/opsxj ci` y ejecuta los dos primeros comandos. `npm audit --omit=dev --json` reportó cero vulnerabilidades de producción; Mermaid y jsdom son dependencias exclusivas de desarrollo.

## Límite de la validación

Estas pruebas validan existencia, sintaxis y correspondencia estructural entre diagramas y código. No demuestran por sí solas fidelidad conductual completa, seguridad criptográfica integral, cobertura de todos los escenarios ni corrección de componentes futuros.

## Pendientes y elementos no verificables

- No existe endpoint/controller/ASMX, por lo que verbo, ruta, autorización y payload HTTP son “No aplica”.
- No existe service de orquestación ni implementación de repositorio; solo se verifican sus interfaces.
- No existe transporte SMTP, resolución real de destinatarios ni finalizador legacy.
- No se puede verificar un flujo completo de login, cookie, reenvío, persistencia o concurrencia con DOC-91.
- Los estados están declarados, pero sus transiciones no están implementadas en esta entrega.
- Los DTO no fuerzan obligatoriedad o validación en setters; sus restricciones dependen de futuros productores.
- No se ejecuta E2E autenticado ni acceso a base de datos para esta revisión documental.
- La instalación informó 11 hallazgos en dependencias de desarrollo (1 bajo, 8 moderados y 2 altos). `npm audit --omit=dev` confirmó cero hallazgos de producción, pero el detalle del audit completo no pudo refrescarse porque el endpoint del registro no respondió; debe revisarse antes de actualizar estas herramientas.
