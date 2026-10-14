# Jira Context - DOC-104

## Summary

PROCESAMIENTO-SERVIDOR

## Description

> Prompt 06 — Procesamiento servidor, temporales y transparencia del grafo
> Contexto de ejecución OPSXJ
> Este prompt corresponde a una Jira independiente. Ejecuta opsxj:new al iniciar para crear su OpenSpec autónomo. Verifica el contrato frontend real del Prompt 05 antes de implementar, sin reutilizar su cambio OpenSpec.
> Rol esperado
> Actúa como arquitecto backend senior especializado en VB.NET, iTextSharp, archivos, concurrencia, integridad documental y manejo de imágenes.
> Fuentes obligatorias
> Exploración técnica y métricas vigentes.
> 
> webservice/WebService_itext.asmx.vb, service_anotate_pdf_image(...).
> 
> itextshare/Class_ItexShare.vb, add_anotate_image_pdf(...), Create_file_pagina(...) y resolución de rutas.
> 
> workflow/Class_transparent.vb completo.
> 
> DTO Class_itext_anotate, auditoría y validación de firma digital.
> 
> Contrato y evidencia reales del Prompt 04: política adaptativa RANGE/STREAM, clasificación por tamaño y /Linearized, y separación entre bytes únicos y bytes físicos.
> 
> Objetivo
> Reducir aperturas/copias innecesarias y eliminar colisiones de temporales, manteniendo idéntico resultado visual, validación de firma, auditoría y reemplazo seguro del documento.
> Caracterización obligatoria previa
> Antes de refactorizar, crea fixtures PDF descartables y registra huellas antes/después para:
> página normal y rotada;
> 
> grafo desde archivo y base64;
> 
> transparencia activada/desactivada;
> 
> distintas escalas y posiciones límite;
> 
> documento firmado digitalmente;
> 
> documento linealizado descartable, registrando la presencia de /Linearized antes y después del pegado de grafo con iTextSharp;
> 
> error antes y después de crear temporal;
> 
> dos operaciones concurrentes del mismo usuario y de usuarios diferentes.
> 
> Implementación obligatoria
> Usar nombres temporales únicos por operación/documento; no reutilizar usuario.png, usuario_1.png, _<nombre>.pdf ni limpiar indiscriminadamente el directorio compartido.
> 
> Liberar Bitmap, PdfReader, PdfStamper, streams e imágenes mediante Using/Finally, incluso ante error.
> 
> Escribir a un temporal completo, validar que pueda abrirse y reemplazar el original de forma segura; conservar mecanismo de recuperación ante fallo de copia.
> 
> Evitar la segunda apertura/extracción de página si el frontend puede refrescarla mediante el contrato de lectura vigente; si sigue siendo necesaria, medirla y aislarla.
> 
> Corregir TransparentAsync a partir de pruebas: no llamarlo asíncrono si no lo es y no introducir un ciclo GetPixel/SetPixel por píxel como solución de rendimiento.
> 
> Implementar transparencia eficiente con acceso bloqueado al bitmap o mecanismo equivalente compatible, preservando tolerancia y alfa.
> 
> Validar dimensiones, página, rotación, escala y tamaño del payload antes de abrir iText.
> 
> Mantener la validación de certificado antes de cualquier escritura y retornar errores públicos saneados.
> 
> No registrar rutas, base64 ni detalles sensibles en la auditoría; revisar el texto actual datos_add_grafo y conservar solo información permitida.
> 
> Clasificar el PDF final por su estructura real después del reemplazo. Si iTextSharp elimina /Linearized, registrar el cambio y asegurar que la siguiente apertura use el fallback STREAM; no publicar una expectativa falsa de descarga parcial.
> 
> Mantener desacopladas la mutación documental y la estrategia de transporte: el guardado del grafo produce un PDF válido y el visor decide RANGE/STREAM al leerlo. No ejecutar linealización externa dentro del WebMethod ni después de firmar.
> 
> Restricciones y antirregresión
> No actualizar iTextSharp ni cambiar de biblioteca en esta Jira.
> 
> No agregar tablas/columnas, colas o almacenamiento externo.
> 
> No agregar Ghostscript, ejecutables externos ni una dependencia de linealización en producción; la POC del Prompt 04 no se convierte en requisito de este ticket.
> 
> No cambiar el modelo de almacenamiento ni exigir conservar /Linearized si iTextSharp reescribe válidamente el documento; la garantía obligatoria es clasificar el resultado y aplicar el fallback seguro.
> 
> No modificar PDFs reales no descartables durante pruebas.
> 
> No considerar éxito hasta confirmar archivo final válido.
> 
> No degradar firma digital, rotación, coordenadas o transparencia.
> 
> Pruebas obligatorias
> Comparación visual/estructural de salida por fixture y página.
> 
> Comparación de /Linearized antes/después con fixture descartable: si se conserva, validar que la política puede elegir RANGE; si se pierde, validar que el visor elige STREAM sin error, rangos redundantes ni afirmación de descarga parcial.
> 
> Fallos en cada etapa con limpieza de temporales y original intacto.
> 
> Concurrencia controlada sin colisiones ni contaminación cruzada.
> 
> Archivo grande con medición por fase: imagen, transparencia, stamper, validación, reemplazo y refresco.
> 
> E2E mutadora autorizada con una sola persistencia y control antes/después.
> 
> Regresión de otras funciones de Class_ItexShare afectadas por helpers compartidos.
> 
> Criterios de aceptación
> Temporales aislados y eliminados de forma determinista.
> 
> Original preservado ante cualquier fallo previo al reemplazo.
> 
> Resultado documental equivalente y tiempo medido menor o justificado.
> 
> El modo de carga posterior al pegado refleja el PDF realmente persistido; la pérdida de linealización no rompe visualización, scroll ni navegación y queda cubierta por el fallback STREAM.
> 
> No se modifica almacenamiento ni se introduce linealización productiva como condición para cerrar esta Jira.
> 
> Errores y auditoría no filtran información sensible.
> 
> Documentación y entrega
> Documenta transacción de archivos, rollback, recursos, concurrencia, métricas y pruebas en Doc/Actualizacion/VisorPdf/Implementacion/<JIRA>/.
> Correcciones opsxj:prompt-review
> Estas reglas fueron agregadas desde opsxj:prompt-review para cubrir hallazgos estructurales corregibles. Deben ajustarse al contexto real del ticket antes de enviar a implementacion.
> Rol esperado
> Definir el rol tecnico esperado para ejecutar el ticket.
> Objetivo
> Describir el objetivo funcional y tecnico verificable.
> Restricciones criticas
> No introducir cambios fuera del alcance declarado.
> 
> No romper comportamiento existente ni contratos publicos.
> 
> Criterios de aceptacion
> El comportamiento implementado cumple el flujo esperado y queda validado con evidencia.
> 
> Contexto obligatorio
> Listar archivos, modulos, servicios, hooks, adapters y documentacion que deben leerse antes de implementar.
> Pruebas obligatorias
> Ejecutar pruebas focales, MSBuild del proyecto Web Forms afectado y E2E con Playwright cuando el flujo lo requiera; registrar comandos y resultados.
> Documentacion tecnica
> Actualizar el paquete documental canonico del ticket.
> Entregable final
> Entregar codigo, pruebas, documentacion, diagramas y evidencia coherente con lo realmente implementado.
> Precondiciones de rutas
> Inventariar en design.md las firmas reales de webservice/WebService_itext.asmx.vb, itextshare/Class_ItexShare.vb y workflow/Class_transparent.vb antes de modificarlas.
> 
> Mantener el WebMethod como frontera de transporte y la transformación PDF en las clases existentes; no trasladar SQL, sesión o reglas ajenas al servicio.
> 
> Registrar en GestionDocumental-Docuarchi.net.vbproj cualquier archivo compilado nuevo solo si la solución existente realmente requiere crearlo.
> 
> No crear modelos, DTOs, repositorios ni servicios paralelos sin una necesidad demostrada en design.md.
> 
> Agregar regla para [FLOW_DETAIL_REQUIRED]: Flujo paso a paso, secuencia o comportamiento esperado.
> Exigir MSBuild del proyecto Web Forms afectado, pruebas focales del procesamiento y evidencia de concurrencia; registrar el resultado.
> Exigir pruebas focales con la infraestructura existente y archivos PDF descartables; no agregar Vitest ni Testing Library para código VB.NET.
> Declarar que código + E2E + validación autorizada + evidencia saneada son una única unidad de entrega dentro del mismo cambio; no crear una tarea o entrega E2E independiente.
> Reutilizar exclusivamente tools/e2e, su autenticación, configuración, validadores, evidencias y utilidades; prohibir login, arnés, proyecto Playwright, configuración o .env paralelos.
> Antes de autenticar, exigir lectura de AGENTS.md y tools/e2e/AGENT-RUNBOOK.md; ejecutar solo con ambiente, cuentas y datos/tareas descartables expresamente autorizados.
> Exigir secretos efímeros, prohibir exponer/imprimir/persistir credenciales, cookies, tokens y cadenas de conexión, usar verificaciones solo SELECT y conservar evidencia saneada.
> Exigir cobertura E2E, cuando aplique, de autorización/control de acceso, lectura sin mutación, escrituras autorizadas, concurrencia y regresión relacionada.
> Respetar feature flags, gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; no cerrar sin validación autorizada, registrar bloqueo explícito y prohibir mocks, simulaciones, resultados inventados y evidencia ficticia.

## Metadata

- Tipo: Tarea
- Prioridad: Medium
- Labels: PROCESAMIENTO, SERVIDOR
