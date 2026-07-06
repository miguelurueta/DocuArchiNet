# PROMPT DE DEUDA TÉCNICA - Frontend Radicación

# TD-FE-04 - Consolidación de Navegación, Tabs y Eliminación de UI de Prototipo

---

# Ticket Asociado

```text
SCRUMCORE-293
```

Este prompt queda asociado a `SCRUMCORE-293` para trazabilidad, rollback y auditoría del ajuste.

---

# Contexto Arquitectónico

Esta deuda técnica debe implementarse respetando las decisiones previamente adoptadas:

- TD-FE-01 → Single Source of Truth para carga de datos.
- TD-FE-02 → Contexto documental único.
- FE-06 → Startup Guard como Composition Root del módulo.

Esta fase no introduce funcionalidades nuevas.

Su objetivo es eliminar restos de implementación temporal y consolidar la navegación definitiva del módulo.

---

# Objetivo

Eliminar deuda técnica relacionada con navegación, tabs y elementos de UI heredados del prototipo, dejando el módulo preparado para la integración completa de los flujos documentales.

---

# Problema Actual

Actualmente existen elementos propios de una implementación provisional:

```text
Tabs con keys técnicas.

↓

console.log()

↓

Navegación desacoplada del Router.

↓

Strings hardcodeados.

↓

Datos mock visibles.

↓

UI que puede inducir al usuario a interpretar información ficticia como real.
```

Esto genera:

- deuda técnica;
- navegación inconsistente;
- difícil mantenimiento;
- riesgo de errores futuros;
- experiencia de usuario poco confiable.

---

# Evidencia Actual

```text
src/modules/radicacion/hooks/RadicacionTabs.tsx

- usa keys "1", "2", "3", "4"
- ejecuta console.log(key)
- renderiza CapDocument directamente
```

```text
src/app/routes/routes.tsx

- solo registra path "radicacion"
- no existen rutas hijas de registro/documentos
```

```text
src/modules/radicacion/components/CapDocument.tsx

- muestra CAPDOCUMENT como gabinete/radicado
- muestra "Documentos: 4"
- muestra archivos ficticios como "Factura.pdf"
```

```text
src/modules/radicacion/components/Modalpendiente.tsx

- muestra radicados, remitentes, trámites y fechas mock
- los datos parecen información real para el usuario
```

---

# Objetivos Arquitectónicos

Consolidar la navegación del módulo mediante:

- rutas semánticas;
- tabs sincronizados o preparados para sincronizarse con navegación;
- constantes de rutas;
- eliminación de datos mock visibles;
- eliminación de código temporal.

---

# Tabs

Las keys deben representar el dominio funcional.

Reemplazar:

```text
1
2
3
4
```

por:

```text
ia

radicacion

documentos

gestion-radicados
```

Las tabs no deben depender de índices numéricos.

---

# Navegación

Eliminar:

```ts
console.log(key)
```

La navegación debe quedar preparada para integrarse con React Router.

Las tabs deben representar el estado de navegación y no mantener navegación paralela.

Si las rutas hijas aún no existen, dejar constantes, helpers o adapters y preparar la sincronización Router ↔ Tabs sin forzar navegación definitiva.

---

# Rutas

Centralizar las rutas del módulo.

Objetivo:

```text
/dashboard/radicacion

/dashboard/radicacion/registro/:idEstadoRadicado

/dashboard/radicacion/registro/:idEstadoRadicado/documentos
```

Si la implementación completa no corresponde a esta fase, crear:

- constantes;
- helpers;
- adapters;

para evitar strings hardcodeados.

No duplicar rutas en componentes.

---

# Relación con Guard Documental

TD-FE-02 define `RadicacionDocumentosGuard` como dueño de la regla documental.

TD-FE-04 no debe reimplementar ni duplicar esa regla dentro de Tabs.

RadicacionTabs puede:

- reflejar estado habilitado/deshabilitado derivado del Context o del Guard;
- redirigir usando helpers de ruta;
- evitar que el usuario entre visualmente a Documentos cuando no corresponde.

RadicacionTabs no debe:

- decidir reglas documentales propias;
- consultar backend;
- inferir estado documental desde consecutivos, gabinetes o ids sueltos.

---

# UI de Producción

## CapDocument

No debe mostrar:

- nombres ficticios;
- documentos ficticios;
- gabinetes ficticios;
- consecutivos falsos.

Mientras no exista integración real deberá utilizar:

- placeholder;
- estado vacío;
- mensaje funcional.

Nunca datos simulados visibles.

No modificar la integración interna del digitalizador salvo lo mínimo necesario para evitar que se inicialice con contexto mock o inválido.

CapDocument no debe inicializar `DigitalizacionDocumentalWorkspace` con valores ficticios como:

```text
CAPDOCUMENT
Factura.pdf
Documentos: 4
```

---

## ModalPendiente

Mientras FE-05 no esté implementado:

Debe mostrarse como funcionalidad pendiente o estado no disponible.

No debe presentar datos mock interpretables como datos reales.

Puede abrir con:

- estado vacío;
- mensaje funcional;
- indicación de funcionalidad pendiente;
- controles deshabilitados si aplica.

Los fixtures solamente podrán utilizarse en:

- Storybook;
- pruebas;
- desarrollo controlado.

Nunca en runtime de producción.

---

# Responsabilidades

## Tabs

Responsables únicamente de representar la navegación.

No contienen reglas documentales.

No contienen lógica de negocio.

---

## Router

Responsable de resolver la navegación.

No los Tabs.

---

## CapDocument

Debe representar únicamente estados válidos.

No inventa información.

---

## ModalPendiente

Debe representar el estado real de disponibilidad de la funcionalidad.

---

# Restricciones

Eliminar:

- console.log()
- datos mock
- navegación temporal
- keys numéricas
- strings duplicados

No implementar:

- backend;
- AppTable;
- pendientes;
- digitalización;
- upload;
- reglas documentales duplicadas;
- rutas definitivas si aún no corresponde por fase.

---

# Principios Arquitectónicos

Aplicar:

- Domain Driven Naming.
- Single Source of Truth para navegación.
- Separation of Concerns.
- Clean Architecture.
- Backward Compatibility.
- No Prototype Code in Production.

---

# Testing

## Unitarios

Validar:

- keys semánticas;
- rutas centralizadas;
- helpers de rutas;
- ausencia de datos mock visibles;
- ausencia de console.log().

---

## Integración

Validar:

- sincronización Router ↔ Tabs cuando las rutas estén disponibles;
- preparación de sincronización Router ↔ Tabs cuando las rutas definitivas aún no existan;
- bloqueo de Documentos cuando corresponda;
- CapDocument no inicializa digitalización con contexto mock o inválido;
- ModalPendiente no muestra tabla mock en runtime.

---

## Regresión

Validar:

- navegación;
- build;
- lint;
- TypeScript;
- ausencia de console.log();
- ausencia de datos mock visibles.

---

# Criterios de Aceptación

- No existen console.log() en runtime.
- Las tabs utilizan nombres de dominio.
- Las rutas están centralizadas o existe una capa preparada para centralizarlas.
- No existen strings duplicados de rutas.
- CapDocument no muestra información ficticia.
- CapDocument no inicializa digitalización con contexto ficticio.
- ModalPendiente no muestra datos mock.
- Las reglas documentales no se duplican en Tabs.
- El módulo queda preparado para la navegación contextual.

---

# Entregables

1. Archivos modificados.

2. Resumen técnico:

- antes vs después;
- consolidación de navegación;
- eliminación de código temporal;
- preparación para fases futuras.

3. Resultado de pruebas.

4. Riesgos residuales.

5. Próximas fases habilitadas por esta limpieza.

---

# Instrucción Final

Consolidar la navegación y la interfaz del módulo de Radicación eliminando código y elementos propios del prototipo, adoptando nomenclatura de dominio, centralizando las rutas, eliminando datos mock y dejando el módulo preparado para la integración completa de los flujos documentales sin introducir breaking changes ni regresiones.
