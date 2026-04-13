# Arquitectura Maestra: AppTabs en GestionRespuesta

## Objetivo

Definir el contexto visual y funcional para reemplazar el contenido actual de `GestionRespuesta.tsx` por un layout basado en `AppTabs`, manteniendo el `AppButton` de "Volver a la bandeja" y preservando la arquitectura visual de los componentes relacionados.

## Alcance

Aplica a:

- Vista `src/modules/gestionCorrespondencia/pages/GestionRespuesta.tsx`
- Componente `AppTabs` como contenedor principal de secciones
- `AppButton` de "Volver a la bandeja"
- Estilos locales del modulo `gestionCorrespondencia`

No aplica a:

- Cambios de negocio o logica de API
- Refactors globales de routing
- Modificaciones del componente `AppTabs` en si

## Resumen de arquitectura

Frontend

- `GestionRespuesta.tsx`: orquesta layout, tabs y accion de volver
- `AppTabs`: wrapper de AntD `Tabs` con estilo enterprise
- `AppButton`: accion primaria para volver a la bandeja
- Contenido por tab: componentes existentes o secciones embebidas

Backend

- Sin cambios

## Principios

- Mantener estructura visual enterprise ya establecida
- Separacion de responsabilidades (contenedor decide items)
- Tab activo controlado desde el contenedor
- Accesibilidad y navegacion por teclado intactas
- No romper rutas ni callbacks existentes

## Contexto de reemplazo

Requerimiento clave:

- Reemplazar el contenido actual de `GestionRespuesta.tsx` por `AppTabs`
- Mantener visible el `AppButton` de "Volver a la bandeja"

Regla visual:

- `AppButton` debe estar fuera del contenido de tabs para evitar que cambie al navegar
- `AppTabs` debe ocupar el area principal del contenido

## Layout objetivo

Estructura sugerida:

- Header/Toolbar del modulo (si existe)
- Boton "Volver a la bandeja"
- `AppTabs` con items definidos por la vista

## Contrato de tabs

Definir items con la forma `AppTabItem`:

- `key`: string unica
- `label`: texto del tab
- `children`: contenido renderizado
- `disabled?`: bloqueo condicional

Reglas criticas:

- No mezclar `activeKey` y `defaultActiveKey`
- No permitir cambio si el tab esta `disabled`

## Estructura de contenido

Definir un arreglo de tabs con contenido basado en las secciones actuales de `GestionRespuesta`.

Ejemplo base:

```tsx
const items: AppTabItem[] = [
  {
    key: "respuesta",
    label: "Respuesta",
    children: <RespuestaPanel />,
  },
  {
    key: "historial",
    label: "Historial",
    children: <HistorialPanel />,
  },
];
```

## Accion "Volver a la bandeja"

Reglas:

- Mantener texto y comportamiento existentes
- Colocar el boton antes de los tabs o en un wrapper superior
- No moverlo dentro de `children` de un tab

## Estilos

- Reusar estilos actuales del modulo `gestionCorrespondencia`
- No crear estilos globales
- Si se requiere ajuste, crear clase local en:
  - `src/modules/gestionCorrespondencia/style/GestionCorrespondencia.module.css`
  - o `src/modules/gestionCorrespondencia/style/GestionRespuesta.module.css` si existe

## Accesibilidad

- `AppTabs` ya hereda ARIA de AntD
- Boton debe mantener `aria-label` si lo tenia
- Foco visible en tabs y en boton

## Responsive

- Tabs horizontales en desktop
- Scroll horizontal en mobile si excede ancho
- Boton siempre visible sin desbordes

## Errores a evitar

- Insertar el boton dentro del contenido de tabs
- Mezclar estado controlado/no controlado en `AppTabs`
- Reemplazar estilos existentes sin revisar dependencias
- Cambiar rutas o handlers del boton

## Plan sugerido

1. Identificar el contenido actual de `GestionRespuesta.tsx`
2. Mapear ese contenido a `AppTabItem[]`
3. Insertar `AppButton` fuera de `AppTabs`
4. Ajustar estilos locales si es necesario
5. Validar layout en desktop y mobile

## Pruebas minimas

- Renderiza `AppTabs` con al menos 2 pestañas
- El boton "Volver a la bandeja" permanece visible al cambiar de tab
- `onChange` ejecuta el cambio de tab
- Tab `disabled` no permite navegar
- Layout responsive sin overflow inesperado
