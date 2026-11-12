# DOC-119 — decisión preparada sobre cobertura de SEG-02

Estado: **propuesta pendiente de decisión expresa del usuario responsable**. No modifica todavía diseño/spec/tareas ni abre la compuerta UI. Origen: D-01/D-03/D-09, RQ-01/RQ-03/RQ-09 y tareas2.5/2.7. El resultado original seguirá siendo PASS_READONLY_WITH_LIMITS cualquiera que sea la decisión.

## Cobertura disponible para revisar

| Comportamiento | Evidencia real | Evidencia local |
| --- | --- | --- |
| Filtros, segunda página, CSV y privacidad | SEG-01 ccb25ec9, PASS_READONLY, huellas iguales | Regresión cliente/políticas |
| Recarga, referencia original y reselección por recurso sin envío | SEG-02 29623218, comprobado, huellas iguales | Chromium con cliente real/servicios sintéticos |
| Relación existente única, selección y expansión conservadas | Conteos lógicos1→1, destino7 con selección40 | HTML WebForms imagen/texto, rama visible y oculta |
| Inserción de relación ausente | No observada: la relación ya existía al recargar | Comprobada por IDs ausentes con cliente/árbol reales y servicio sintético |
| Raíces físicas homónimas | No observadas en el lote29 | Comprobadas con dos raíces homónimas de IDs distintos y reselección del recurso explícito |
| Duplicado lógico real | No observado en el lote29 | Rechazado por el verificador; enlaces imagen/texto se distinguen de nodos distintos |

Las fuentes reales son `doc119-seg01-aceptado-20261112.json` y `doc119-recuperacion-verificada-20261112.json`; la regresión local28/28 cliente y49/49 políticas consta en `doc119-recuperacion-arbol-20261112.md`. Ningún caso sintético se reclasifica como IIS/MySQL real.

## Opción preparada: aceptar cobertura mixta y avanzar

Aceptar SEG-02 para la compuerta operativa con los controles reales anteriores y las regresiones locales complementarias de inserción/homónimos, conservando ambos límites en evidencia y guía final. La aceptación responsable será **ACEPTADO_CON_COBERTURA_MIXTA**; no cambia los JSON de ejecución ni afirma que todos los casos se observaron en IIS.

Si el usuario confirma esta opción, registrar su decisión y esta matriz como criterio explícito de aceptación por capas en D-03/D-09 y la verificación de2.5/2.7; completar ambas tareas y avanzar a9/24. Las obligaciones funcionales de RQ-03 se mantienen: IDs, autoridad actual, reselección, ausencia de duplicados y conservación de selección/expansión. La reorganización UI continuará con las regresiones previstas en el diseño, preservando la distinción entre cobertura real y local. No se autoriza por esta decisión crear más datos, publicar, modificar permisos ni limpiar.

## Alternativa: exigir ambos casos adicionales en IIS

Mantener2.5/2.7 abiertas y preparar una ampliación concreta de recursos/efectos para revisión antes de ejecutar. No basta repetir el lote29: recargar conserva la relación ya existente y no cambia los nombres físicos de las raíces. Con el perfil40, raíces con el mismo nombre derivan el mismo código y se bloquean por coincidencia; resolver ese escenario exige un conjunto válido y una decisión de diseño/prueba, sin retirar esa protección ni fabricar una inserción eliminando nodos del DOM.

La aprobación de esta propuesta es una decisión de suficiencia de cobertura. El silencio, continuar escribiendo documentación o un commit no la sustituyen. Hasta recibirla, progreso7/24 y compuerta UI cerrada.
