# 01 — Backend de consulta y descarga segura

## Rol esperado

Arquitecto y desarrollador senior VB.NET con experiencia en ASP.NET Web Forms, contratos ASMX/HTTP, MySQL, autorización y entrega segura de archivos.

## Objetivo

Separar la consulta y descarga de autorizaciones de los controles Web Forms y ofrecer contratos modernos de solo lectura, parametrizados, paginados, autorizados y saneados.

## Contexto obligatorio

- Leer `00-contexto-obligatorio.md` y las fuentes allí indicadas.
- Requiere aprobación de todas las decisiones funcionales enumeradas en `README.md`.
- Inventariar primero tabla, claves, estados, joins, índices, generadores de XML/consolidado y todos los consumidores del dominio legacy.
- No implementar interfaz productiva ni retirar consumidores en esta etapa.

## Requisitos positivos

- Crear DTOs, servicio de aplicación y puerto de repositorio independientes de `Label`, `GridView`, `UpdatePanel`, `Page`, sesión y controles ocultos.
- Exponer en la infraestructura moderna existente una consulta con `IdTarea`, filtros permitidos, cursor o página y tamaño limitado; devolver orden estable por fecha e identificador.
- Resolver el usuario autenticado y validar acceso real a la tarea en servidor. Rechazar tarea inexistente, ajena, inactiva o fuera del contexto permitido sin revelar su existencia.
- Parametrizar todos los valores. Resolver columnas/direcciones de orden mediante un mapa cerrado del servidor.
- Devolver datos mínimos definidos por las decisiones funcionales, capacidades explícitas como `PuedeDescargar` y códigos funcionales saneados.
- Implementar descarga individual y consolidada reutilizando el mecanismo de archivos aprobado del repositorio. Revalidar acceso, pertenencia y estado al descargar; fijar `Content-Type`, `Content-Disposition`, nombre saneado y protección contra cache cuando corresponda.
- Mantener trazabilidad interna saneada de fallos sin escribir auditoría funcional de la tarea por una lectura.
- Preservar temporalmente el adaptador legacy requerido por consumidores aún activos, sin duplicar reglas de autorización.

## Restricciones críticas

- No aceptar nombres de columnas, rutas, nombres de archivo, identidad del usuario o contenido documental desde el cliente.
- No usar la respuesta binaria del ASMX JSON como atajo si el repositorio dispone de un mecanismo oficial de descarga; justificar técnicamente el transporte elegido.
- No modificar, crear, anular ni reactivar autorizaciones.
- No eliminar la clase legacy o controles Web Forms compartidos.

## Criterios de aceptación

- La consulta autorizada es determinista, limitada y no mutante.
- Un identificador cruzado o manipulado no permite enumerar ni descargar otra autorización.
- La descarga llega como adjunto y no navega a XML crudo ni revela rutas internas.
- Errores públicos no contienen excepciones, SQL o información sensible.
- El consumidor legacy preservado conserva su comportamiento hasta la etapa 03.

## Secuencia funcional

1. El cliente envía `IdTarea` y criterios limitados de consulta.
2. El servidor reconstruye sesión, acceso y pertenencia, aplica orden interno y consulta parametrizada.
3. El servicio devuelve únicamente el DTO y las capacidades autorizadas.
4. Para descargar, el cliente expresa tarea y autorización; el servidor repite acceso y pertenencia antes de generar el adjunto.
5. Éxito o error regresan un resultado saneado sin mutar tarea, autorización ni auditoría funcional.

## Pruebas obligatorias

Agregar pruebas focales para anónimo, tarea inexistente/ajena/inactiva, usuario autorizado, estados decididos, vacío, límites, orden estable, filtro parametrizado, orden manipulado, cursor/página inválidos, autorización cruzada, pertenencia de descarga, consolidado, nombre/tipo de archivo y error saneado. Demostrar con controles `SELECT` que listar, detallar y descargar no mutan tarea, autorización ni auditoría.

Integrar la E2E real aplicable conforme a `bloque-e2e-integrado.md`, usando exclusivamente `tools/e2e`. Ejecutarla solo con ambiente, cuenta, tarea y autorizaciones expresamente aprobados; si falta una precondición, registrar bloqueo. Ejecutar MSBuild o la compilación disponible y registrar resultados.

Ejecutar pruebas unitarias/focales VB.NET o CJS según el área afectada y conservar evidencia estructural con comando, resultado y cobertura. Código, E2E, validación autorizada y evidencia saneada forman una única unidad dentro de este cambio; no crear una tarea E2E independiente.

Reutilizar exclusivamente `tools/e2e`, su sesión, configuración, validadores, evidencias y utilidades; no crear login, arnés, proyecto Playwright, configuración ni `.env` paralelos. Antes de autenticar, leer `AGENTS.md` y `tools/e2e/AGENT-RUNBOOK.md`; ejecutar solo con ambiente, cuentas, tarea y autorizaciones expresamente aprobados. Usar secretos efímeros, no imprimir ni persistir credenciales, cookies, tokens o cadenas de conexión, limitar verificaciones a `SELECT` y guardar evidencia saneada.

La cobertura E2E incluye autorización/control de acceso, lectura sin mutación, tarea propia/ajena/inactiva, autorización cruzada, listado y descargas permitidas, y regresión. Escrituras autorizadas y concurrencia mutante no aplican a este recorrido de lectura. Respetar gates, usuarios, grupos y seguridad sin habilitarlos arbitrariamente; si falta autorización o datos, registrar bloqueo explícito sin mocks, simulaciones ni evidencia ficticia.

La E2E es parte integral del mismo cambio funcional y de su criterio de cierre. Antes de autenticar se requieren ambiente, cuentas y datos o tareas descartables expresamente autorizados. No cerrar sin validación autorizada; respetar feature flags, gates, usuarios y grupos sin habilitarlos arbitrariamente.

## Documentación técnica

Actualizar arquitectura, contrato/datos, seguridad/descarga, matriz de pruebas e inventario en el paquete documental canónico del DOC; si aún no existe su ruta, registrar el bloqueo sin crear documentación en la raíz.

## Entregable final

Entregar endpoints, DTOs, reglas, archivos, pruebas unitarias/focales, E2E autorizada o bloqueo, compilación MSBuild disponible, documentación y evidencia saneada, y relevo a 02.
