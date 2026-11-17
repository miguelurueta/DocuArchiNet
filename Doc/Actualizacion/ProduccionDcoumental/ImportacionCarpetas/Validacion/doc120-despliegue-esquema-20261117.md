# DOC-120 — esquema verificado y despliegue local

Estado: **DESPLEGADO EN IIS LOCAL; ACEPTACIÓN DE IMPORTACIÓN PENDIENTE**. Código de producto `af1ff0733a3ed123d8fb198b26a8f665fe799a8f`, posterior a `5a896101`. [Resultado primario saneado y huellas](doc120-despliegue-esquema-20261117.json). Este corte complementa, sin reescribir, la [verificación local anterior](doc120-segmentada-20261117.md).

El usuario informó «aplicado el cambio» después de recibir SQL08 para aplicación manual y luego indicó «continua» ante verificar las tres tablas y desplegar la DLL. Se interpretó como autorización de esa preparación técnica en `https://localhost/GestionDocumental-Docuarchi.net/`; no de carga, fallos transaccionales o creación de datos. **No se volvió a ejecutar DDL**. Las ventanas3.118/3.126/3.127 y5.2 conservan su autorización propia.

## Resultado observado

| Control | Resultado y límite |
| --- | --- |
| Ruta IIS | Aplicación `Default Web Site/GestionDocumental-Docuarchi.net` apunta a la carpeta principal del repositorio. Comparte `DefaultAppPool`: no se reinició el grupo ni otros sitios |
| Esquema real | Run `25b3cd1989384452b39a70282782d9b9`:10 SELECT de metadatos intentados/completados,0 HTTP y0 efectos. Validador del producto acepta esquema anterior y tres tablas nuevas/31 columnas, tipos, índices y FK. Cuenta de lectura introducida en consola; secretos eliminados al terminar |
| Alcance del DSN | RA elegido por el operador. No autentica una sesión de aplicación ni verifica todavía su resolución RA/DA particular |
| Corrección previa | El lector de metadatos incluye índices de `ra_integracion_perfil_version`. El detector nuevo los trataba incorrectamente como índices extra de preparación. Regresión reproduce el rechazo antes del cambio; después pasa y mantiene el rechazo de un índice único extra en las tablas propias. No requiere otro SQL |
| Validación local |11/11 de este corte:2 wrappers Framework/DDL y9 controles de diagnósticos, con5 casos nuevos de esquema. No se suman a los200 históricos. Auxiliar C# acepta los fixtures completos y el índice padre, sin conexión |
| Compilación | Build completo Debug en `.tmp/`, sin publicación de bases ni DeployOnBuild;0 errores. Referencias de ensamblados instaladas compatibles y1 recurso incrustado |
| Instalación | Solo DLL y PDB del sitio, con copia de los anteriores y sustitución atómica. DLL instalada SHA256 `b96be78ed269a0e37c3e55173e30356bd46d5edff5a7e4a9fb87948376d31a00`; coincide con la validada para el esquema |
| Comprobación inicial | Dos GET WSDL devuelven500. Diagnóstico aislado con DLL anterior/nueva reproduce la misma incompatibilidad SOAP de `EnsayarReglas`/`EjemploPerfilImportacion.Expediente` (`IDictionary`). Es preexistente y no es el transporte JSON del módulo; no se cambió ese contrato |
| Canal JSON real | Tercera solicitud técnica, POST anónimo a la nueva consulta `ConsultarPreparacionPlan`:401, `SESION_REQUERIDA`, datos nulos. Comprueba carga del servicio y denegación antes de consulta de recurso; no equivale a una prueba autenticada ni completa el recorrido |
| Negocio |0 recepciones,0 expedientes,0 documentos y0 inicios de sesión autenticados. Sin credenciales de aplicación, cambios de perfil/cuentas o importaciones |

La comprobación WSDL se conserva como intento fallido del chequeo técnico; no se oculta ni se presenta como regresión resuelta del servicio SOAP. El estado final es `PASS_DEPLOYED_JSON_GUARD`, con3 HTTP técnicos acumulados. No se repetirá esta ventana ni el diagnóstico SQL automáticamente.

## Operación y retorno

Recargar el navegador con **Ctrl+F5** carga la interfaz actual y la DLL anuncia preparación segmentada/consulta focal. El acceso autenticado, la revisión de1.000 PDF, el almacenamiento y la recuperación reales siguen sin aceptación en este corte. No se inició esa prueba.

El retorno compatible debe conservar la corrección de `af1ff073` y sus lectores de planes1/2. **Este mínimo sustituye a `d5bae464`**: aquel conserva el falso rechazo de índices cuando se leen metadatos reales. Los archivos anteriores guardados durante el despliegue son evidencia/copia previa; no constituyen un retorno válido para gestionar planes2. No revertir datos/tablas ni instalar lectores anteriores por inferencia. Se conserva el paquete compilado compatible en `.tmp/`, excluido de Git, con huellas en el JSON compañero.

Reproducción de controles: `node --test tests/doc110-perfil-importacion-behavior.test.cjs`; `node --test tests/doc120-schema-readiness.test.cjs tests/doc120-final-state-diagnostic.test.cjs`. La consola real usa `inspect-doc120-schema-readiness.cjs --run <runId-nuevo> --authorized-readonly`; sus flags no sustituyen una nueva instrucción del operador. Preparar/verificar el esquema no ejecuta la [ventana de1.000 PDF](doc120-capacidad-plan-real-20261117.md).
