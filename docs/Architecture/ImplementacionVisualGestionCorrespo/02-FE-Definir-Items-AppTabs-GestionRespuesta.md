# PROMPT ARQUITECTONICO  Ticket 02 FE
# Definir items de tabs y ajustes visuales en GestionRespuesta

Rol esperado:
Arquitecto de software senior frontend (React, componentes UI enterprise, accesibilidad, testing)


OBJETIVO

Definir la estructura final de `items` para `AppTabs` en `GestionRespuesta.tsx` y ajustar estilos locales del modulo para asegurar consistencia visual y responsive, sin introducir estilos globales.


CONTEXTO EXISTENTE

- arquitectura de referencia: `docs/Architecture/ImplementacionVisualGestionCorrespo/GestionRespuesta-AppTabs-Architecture.md`
- contrato de `AppTabs`: `docs/Architecture/AppTabs/AppTabs-Architecture.md`
- estilos del modulo: `src/modules/gestionCorrespondencia/style/`


UBICACION (OBLIGATORIA)

```
src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx
src/modules/gestionCorrespondencia/style/
```


RESTRICCIONES (OBLIGATORIAS)

- no crear estilos globales
- no modificar estilos del componente `AppTabs`
- no agregar dependencias nuevas
- no usar `any`


CONTRATO (OBLIGATORIO)

`items` debe cumplir `AppTabItem`:

- `key`: string unica
- `label`: texto del tab
- `children`: contenido renderizado
- `disabled?`: bloqueo condicional


REGLAS DE IMPLEMENTACION (OBLIGATORIAS)

1. Definir al menos 2 tabs coherentes con el contenido actual
2. Mantener el boton "Volver a la bandeja" visible
3. Ajustes de estilos solo via CSS Modules del modulo
4. Responsive: tabs con overflow horizontal en mobile si aplica


RIESGOS A EVITAR (OBLIGATORIO)

- tabs con keys duplicadas
- estilos globales accidentales
- layouts que rompan en mobile


PRUEBAS UNITARIAS (OBLIGATORIAS)

- `AppTabs` renderiza items definidos
- `disabled` bloquea navegacion (si aplica)


PRUEBAS QT (CALIDAD / E2E)

- tabs visibles y navegables
- layout sin overflow inesperado en mobile


CRITERIOS DE ACEPTACION

- `items` definidos y renderizados en `GestionRespuesta`
- estilos locales ajustados sin tocar estilos globales
- comportamiento responsive correcto
