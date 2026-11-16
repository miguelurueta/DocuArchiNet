# DOC-120 — mensaje claro para revisión grande

Solicitud «pero no muestra el mensaje claro», posterior al video2026-11-16 123321. Base721883254f1b2c3c0f729ef83d3764a7608d9f62, rama feature/DOC-120, carpeta principal. Tareas3.95/3.96; implementación y verificación locales, sin nueva corrida autenticada.

## Comportamiento

El host autorizado publica un metadato numérico: mínimo del límite JSON efectivo ASMX obtenido de configuración y262144 caracteres ya admitidos por el importador. No cambia Web.config ni aumenta límites. El cliente compara el cuerpo serializado de PrepararPlan con ese dato; si lo excede, devuelve REVISION_TAMANO_EXCEDIDO sin crear XHR ni enviar el POST. El aviso incluye cantidad de PDF, caracteres del cuerpo y máximo admitido; conserva selección/perfil y explica que repetir no resuelve la capacidad. La petición detenida localmente no se presenta como revisión persistida.

Sin el metadato, el cliente distingue HTTP413 y errores ASMX estructurados: HTTP5xx, tipo exacto System.ArgumentException/System.InvalidOperationException y texto de longitud excedida con maxJsonLength en español/inglés. Solo se conserva el código cerrado; Message/StackTrace no salen al usuario ni al objeto de error. No se afirma que una revisión no se guardó cuando el servidor no devolvió su resultado.

HTML genérico, mensajes con solo la palabra maxJsonLength, tipos desconocidos, otros códigos HTTP o cualquier error no reconocido mantienen RESPUESTA_INVALIDA, ahora explicado con cantidad y causa no identificada. No se transforma todo HTTP500 de una selección grande en límite JSON. HTTP401/403, aun sin sobre del importador, conservan mensaje neutro, sin cantidad ni datos privados, y cierran el transporte.

Ejemplo con límite comprobado: «La revisión de1.000 PDF supera el tamaño de datos que admite esta importación. Datos de revisión:172.409 caracteres. Máximo permitido:102.400. La solicitud se detuvo antes de enviarla; no se guardó una revisión con este intento. Se conserva tu selección y perfil». Las cifras reales se calculan del cuerpo y del límite publicado; no se fijan por contar archivos.

## Evidencia y límites

**122/122 pruebas Node únicas**, incluidas10 nuevas y2 wrappers Framework/DDL; no se suman casos internos o corridas focales repetidas. Transporte real con XHR sintético, formularios/CSS reales y Chromium en390/768/1280px. Casos:1.000 metadatos, frontera exacta/UTF16, hint ausente/inválido, otros endpoints, señales ASMX/HTTP413, respuestas desconocidas, privacidad, foco y selección/perfil conservados. Regresión de mensajes, cliente, revisión, fidelidad, coordinación y navegación conservada.

Log conjunto: `.tmp/doc120-mensaje-tamano-regresion-final.log`; focal superpuesta: `.tmp/doc120-mensaje-tamano-focal-final.log`. Compilación aislada sin errores: `.tmp/doc120-mensaje-tamano-build/output.log`; advertencias heredadas fuera del cambio. Capturas sintéticas: `.tmp/doc120-preparacion-tamano/tamano-390.png`, `tamano-768.png`, `tamano-1280.png`. OpenSpec estricto y diff verificados. JSON compañero fija fuentes LF y artefactos por SHA256.

La reproducción anterior de la selección local real produce172409 caracteres frente al límite ASMX predeterminado102400. El video solo muestra una respuesta no reconocida: este corte no observó el cuerpo HTTP ni la configuración efectiva de la corrida filmada. El nuevo hint permite comprobar el límite efectivo cuando la DLL actualizada esté desplegada; no se hizo ese despliegue/activación IIS aquí. Servidores anteriores conservan la clasificación segura del error estructurado; una respuesta opaca seguirá diciendo que no se identificó su causa.

Sin autenticación, HTTP de aplicación, SELECT o efectos reales nuevos. No se modifican inventarios, perfiles, Web.config, esquema, SII, cuotas, almacenamiento o reanudación. Las selecciones grandes siguen sujetas al tamaño del resultado de revisión y al costo de relecturas: este cambio no resuelve ni declara capacidad para importar1.000 documentos completos.

Avance **110/116 tareas**,41 escenarios,18 S/98 M. Pendientes3.86/3.87/5.2/6.4/6.5/6.6; sin aceptación integral ni archivo de DOC-120.
