# DOC-117 — Observación interna ALM-03, desarrollo local aprobado

El usuario aprobó mediante «aprobado» la excepción editorial propuesta en `doc117-alm03-observacion-propuesta-20261106.md`. Se aplicó a D-06/D-08 y al escenario de observación de RQ-06. La aprobación cubre desarrollar y probar la copia de laboratorio; no concede instalación o efectos reales. Progreso **16/18**; ALM-03 de aplicación continúa NO_EJECUTADO.

## Resultado local

- Regresión conjunta **184/184 PASS**, incluidas 50 pruebas de observación/candidata/restauración ALM-03. No se suman otra vez los casos internos del ejecutable .NET al total.
- Candidata desactivada compilada con el proyecto completo, fuera de bin, sin errores. SHA-256: `d2003810f3395eb21eb096e153bb5245bb599f673844bdfda4ba1e75c5a86cd6`.
- Las 912 entradas originales de compilación/recursos conservaron sus huellas y el proyecto mantuvo su estado previo. No se modificó `workflow/ClassAlmacenamiento.vb` ni se agregó la fixture al proyecto productivo.
- DLL instalada conservada: `4134f57f66fc259b5f33a553577c9a07e6bf176b2da027aaeeb8576325c282c5`.
- Ensayo exclusivo de copia de bytes en `.tmp`: guardián independiente restauró tras salida del propietario sin finally, ambos respaldos idénticos y transferencia de ACL verificada. Sin IIS, carga de DLL, aplicación autenticada o base de datos.

La compilación inicial encontró un error local de resolución de `Count`; se corrigió usando Enumerable.Count y se recompiló en otro directorio. Las correcciones posteriores del núcleo se incluyeron en la candidata final. Los reportes de compilaciones anteriores permanecen históricos y no se presentan como la candidata final.

## Instrumentación y evidencia

`tools/e2e/scripts/support/doc117-alm03-candidate.cjs` genera una copia con dos adiciones dentro de `AlmacenamientoDocumentoProduccionDocumental`: entrada antes del cuerpo original y salida en un finally exterior. Eliminar esas adiciones reproduce exactamente el texto fuente original; se conservan sin editar las instrucciones originales, sus argumentos y ByRef. La salida recibe el resultado e IDs originales. El generador rechaza fuentes divergentes, destinos fuera de `.tmp` y directorios reutilizados.

`tools/e2e/fixtures/Doc117Alm03MethodObserver.vb` contiene el núcleo de observación. Preparación, comprobación de reserva e identidad de sesión preceden a la toma de tiempo. Las reservas y marcas usan CreateNew/Flush(True); se rechazan duplicados o reservas alteradas. El contador es monotónico, con dominio de reloj, proceso, AppDomain, hilo administrado/nativo y huella de recurso. Las sesiones se comparan exclusivamente en memoria; ningún SessionID se serializa.

El núcleo no mantiene un bloqueo durante el escritor. El sellado final solo ocurre después de que las tres salidas hayan terminado de persistirse o registrado su fallo. Fallar al guardar una salida no reemplaza el retorno o excepción originales: deja evidencia incompleta y bloquea la aceptación. No se reintenta el efecto para obtener una marca.

Las pruebas .NET compilan la misma envoltura sobre un escritor sintético y una copia del núcleo con una entrada de prueba, únicamente bajo `.tmp`. Verifican éxito/ByRef, Return temprano, rechazo capturado, excepción propagada, fallos de entrada/salida a disco, entrada duplicada, reserva alterada y rechazo de sesiones iguales/cambiadas. La barrera que hace determinista el solapamiento pertenece al escritor doble de prueba; no existe Sleep/barrera/espera entre participantes en el observador ni en la envoltura de la candidata.

`doc117-alm03-method-observation.cjs` exige seis marcas y un resumen completo para IMPORT_A, IMPORT_B e INDIVIDUAL. Solo compara el par A/individual; la segunda importación debe empezar después. Rechaza intervalos externos, otros recursos/ejecuciones/relojes/hilos, IDs repetidos, retornos fallidos, sesiones no verificadas y evidencia incompleta. Distingue METHOD_BODIES_OVERLAPPED de SERIALIZED_METHODS. Incluso un intervalo interno positivo mantiene `businessEvidenceVerified=false` y `alm03ConcurrentPass=false` hasta contar con los controles físicos y de ejecución real independientes.

## Límites y trabajo que falta

La entrada de la candidata final **siempre rechaza** con `DOC117_ALM03_DORMANT_SCOPE` antes del cuerpo documental. No contiene fábrica de alcance activo, selector por entorno, endpoint o header. Se probó el rechazo de esa entrada original antes de compilar la copia sintética de prueba. Esta candidata no está lista para instalar.

Faltan el preflight autorizado del destino individual y de los recursos nuevos, su preparación aprobada y el binding activo de los llamadores. Ese binding deberá comprobar el comando inmutable/snapshot/solicitud de importación y el contexto oficial de la carga individual, además de vincular exactamente los argumentos del escritor a los tres recursos sellados y dos sesiones. El núcleo probado no sustituye esas guardas. No se han generado ni usado recursos/huellas reales ALM-03.

Tras cerrar esos datos, será necesario revisar una candidata específica, un ejecutor y una autorización real distintos. Los lotes 18–21 e intentos 41/52/53 se conservan; B de ALM-02 sigue fuera de cualquier repetición/reparación. No hubo login, SELECT real, recepción, expediente, documento o instalación nueva durante este desarrollo.

Las conclusiones futuras se limitarán al tiempo de vida del método instrumentado, incluidos sus bloqueos; no a commits simultáneos, rendimiento del binario ordinario, XML o SII. La observación puede perturbar los tiempos. No se completa 17.1/18.1 ni se archiva DOC-117 con estos resultados locales.

Huellas, compilación, regresión y restauración: `doc117-alm03-metodo-local-20261106.json`.
