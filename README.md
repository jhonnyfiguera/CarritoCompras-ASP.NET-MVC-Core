# Carrito de Compras 📖

## Objetivos 📋

Desarrollar un sistema que permita la administración del stock de productos a una PYME que tiene algunas sucursales de venta de ropa.\
Los empleados gestionan Empleados, Clientes, Productos, Categorias, Compras, Carritos, Sucursal, StockItem, etc.\
Permitir a los clientes realizar compras online.\
Utilizar Visual Studio 2019 y crear una aplicación utilizando `ASP.NET MVC Core versión 3.1`.

---------------------------------------

## Enunciado 📢

La idea principal de este trabajo práctico, es que ustedes se comporten como un equipo de desarrollo. Este documento, les acerca un equivalente al resultado de una primera entrevista entre el cliente y alguien del equipo que relevó e identificó la información aquí contenida. A partir de este momento, deberán comprender lo que se está requiriendo y construir dicha aplicación.\
Deben recopilar todas las dudas que tengan y evacuarlas con su nexo (el docente) de cara al cliente. De esta manera, él nos ayudará a conseguir la información ya un poco más procesada. Es importante destacar que este proceso no debe esperar a ser en clase, sino que debe darse a medida que vayan trabajando en el proyecto. Por otro lado es importante que agrupen sus consultas ya sea por criterios funcionales o técnicos y envíen correos con las consultas agrupadas en lugar de enviar cada consulta de forma independiente.

### Consultas

Las consultas que sean realizadas por correo a mailto:federico.marchese@ort.edu.ar deben seguir el siguiente formato:

**Subject:**

- `[NT1-<CURSO LETRA>-GRP-<GRUPO NUMERO>] <Proyecto XXX> | Informativo o Consulta` *ej: [NT1-A-GRP-5] Agenda de Turnos | Consulta*

**Body:**

1. `<xxxxxxxx>` *ej: ¿La relación del paciente con Turno es 1:1 o 1:N?*
2. `<xxxxxxxx>` *ej: ¿Está bien que encaremos la validación del turno activo, con una propiedad booleana en el Turno?*

---------------------------------------

## Proceso de ejecución en alto nivel ☑️

- Crear un proyecto utilizando [visual studio].
- Adicionar todos los modelos dentro de la carpeta Models cada uno en un archivoseparado.
- Especificar todas las restricciones y validaciones solicitadas a cada una de lasentidades. [DataAnnotations].
- Crear las relaciones entre las entidades.
- Crear una carpeta Data que dentro tendrá al menos la clase que representará elcontexto de la base de datos DbContext.
- Crear el DbContext utilizando base de datos sqlite. [DbContext], [Database Sqlite],[Db browser sqlite].
- Agregar los DbSet para cada una de las entidades en el DbContext.
- Crear el Scaffolding para permitir los CRUD de las entidades.
- Aplicar las adecuaciones y validaciones necesarias en los controladores.
- Realizar un sistema de login para los roles identificados en el sistema y unadministrador.
- Un administrador podrá realizar todas tareas que impliquen interacción del lado del negocio (ABM "Alta-Baja-Modificación" de las entidades del sistema y configuraciones en caso de ser necesarias).
- Cada usuario sólo podrá tomar acción en el sistema en base al rol que tiene.
- Realizar todos los ajustes necesarios en los modelos y/o funcionalidades.
- Realizar los ajustes requeridos del lado de los permisos.
- Todo lo referido a la presentación de la aplicaión (cuestiones visuales).
- Para la visualización se recomienda utilizar [Bootstrap], pero se puede utilizar cualquier herramienta que el grupo considere.

---------------------------------------

## Entidades básicas 📄

- Usuario
- Cliente
- Empleado
- Producto
- Categoria
- Stock
- StockItem
- Carrito
- CarritoItem
- Compra

`Importante: Todas las entidades deben tener su identificador único Id de tipo Guid.`

`Las propiedades descriptas a continuación, son las mínimas que deben tener las entidades, dejando explícito que ustedes pueden agregar las que consideren necesarias.  
De la misma manera deben definir los tipos de datos asociados a cada una de ellas, como así también las restricciones que apliquen.`

| Entidad | Propiedades |
| ----- | ----- |
| Usuario | Nombre, Email, FechaAlta, Password |
| Cliente | Nombre, Apellido, DNI, Telefono, Direccion, FechaAlta, Email, Compras, Carritos |
| Empleado | Nombre, Apellido, Telefono, Direccion, FechaAlta, Email |
| Producto | Nombre, Descripcion, PrecioVigente, Activo, Categoria |
| Categoria | Nombre, Descripcion, Productos |
| Sucursal | Nombre, Direccion, Telefono, Email, StockItems |
| StockItem | Sucursal, Producto, Cantidad |
| Carrito | Activo, Cliente, CarritoItems, Subtotal |
| CarritoItem | Carrito, Producto, ValorUnitario, Cantidad, Subtotal |
| Compra | Cliente, Carrito, Total |

**NOTA:** aquí un link para refrescar el uso de los [Data annotations].

---------------------------------------

## Características y Funcionalidades ⌨️

`Todas las entidades deben tener implementado su correspondiente ABM, a menos que sea implícito el no tener que soportar alguna de estas acciones.`

### Cliente

- Los Clientes pueden auto registrarse.
  - La autoregistración desde el sitio es exclusiva para los clientes, por lo cual, se le asignará dicho rol automáticamente.
- Un usuario puede navegar los productos y sus descripciones sin iniciar sesión, es decir, de forma anónima.
- Para agregar productos al carrito debe iniciar sesión primero.
- El cliente, puede agregar diferentes productos en el carrito y por cada producto modificar la cantidad que quiere.
  - Esta acción no implica validación en stock.
  - El ciente verá el subtotal por cada producto/cantidad.
  - También verá el subtotal del carrito.
- El cliente puede finalizar la compra.
  - Cuando elija finalizar la compra se le solicitará un lugar para retirar (una sucursal).
- El cliente puede vaciar el carrito.
- El cliente puede actualizar sus datos de contacto tales como el teléfono, dirección, etc., pero no puede modificar sus datos básicos tales como DNI, Nombre, Apellido, etc.

### Empleado

- Los empleados, deben ser agregados por otro empleado o administrador.
  - Al momento del alta del empleado se le definirá un username y password.
  - También se le asignará a estas cuentas el rol de Empleado automáticamente.
- El empleado puede listar las compras realizadas en el mes en modo listado, ordenado de forma descendente por valor de compra.
- El empleado puede dar de alta otros empleados.
- El empleado puede crear productos, categorias, Sucursales, agregar productos al stock de cada sucursal.
- El empleado puede habilitar y/o deshabilitar productos.

### Producto y Categoría

- No pueden eliminarse del sistema.
- Solo los productos pueden dehabilitarse.

### Sucursal

- Cada sucursal tendrá su propio stock y sus datos de locación y contacto.
- Por el mercado tan volátil las sucursales pueden crearse y eliminarse en todo momento.
  - Para poder eliminar una sucursal, la misma **no tiene que tener productos en su stock**.

### StockItem

- Pueden crearse pero nunca pueden eliminarse desde el sistema. Son dependientes de la surcursal.
- Puede modificarse la cantidad que se dispone de dicho producto en todo momento a través de la propiedad *Cantidad*.
- Si se elimina la sucursal del stockItem, éste elemento también será eliminado.

### Carrito

- El carrito se crea automáticamente en estado activo con la creación de un cliente (todo cliente tendrá un carrito en estado activo cuando éste sea creado).
- Solo puede haber un carrito activo por usuario en el sistema.
- Un carrito que no esté activo no puede modificarse en ningún aspecto.
- No se pueden eliminar carritos.
- El carrito se desactiva al momento de concretarse una compra de manera automática y se creará un nuevo carrito activo para el usuario.
- Al vaciar el carrito, se eliminan todos los CarritoItems y datos que no sean necesarios.
- El subtotal del carrito es un dato calculado en el momento.

### CarritoItem

- El valor unitario del carritoItem debe actualizarse al realizar cualquier modificación según el precio que tenga vigente cada uno de los productos.
- El subtotal debe ser una propiedad calculada en base a la cantidad de unidades multiplicado por el valor unitario de cada producto.

### Compra

- Al generarse la compra el carrito que tiene asociado pasa a estar en estado *Inactivo*.
- Al finalizar la compra se validará si hay disponibilidad de stock de todos los CarritoItem en la sucursal que seleccionó el cliente.
  - Si hay stock disminuye el mismo en la sucursal seleccionada y se crea la compra.
  - Si no hay stock verificar en otras sucursales si cuentan con el stock de lo que se quiere comprar.
    - Si se encuentran sucursales con stock, sugerir dichas sucursales al cliente o indicar en las que no hay stock.
    - Cuando selecciona una de las sucursales sugeridas con stock, finalizar la compra.
- Al finalizar la compra, se le muestra el detalle de la compra al cliente y se le da las gracias. Se le dá el Id de compra y los datos de la Sucursal que eligió.
- No se pueden eliminar las compras.

### Aplicación General

- Información institucional.
- Se deben mostrar los productos por categoría.
- Los productos que están deshabilitados, deben visualizarse como Pausados. Independientemente, de que haya o no en stock.
- Los accesos a las funcionalidades y/o capacidades, deben estar basadas en los roles que tenga cada individuo.

[//]: # (referencias externas)
   [visual studio]: <https://visualstudio.microsoft.com/en/vs/>
   [Data annotations]: <https://www.c-sharpcorner.com/UploadFile/af66b7/data-annotations-for-mvc/>
   [Bootstrap]: <https://getbootstrap.com/>
   [DbContext]: <https://docs.microsoft.com/en-us/dotnet/api/microsoft.entityframeworkcore.dbcontext?view=efcore-3.1>
   [Database Sqlite]: <https://docs.microsoft.com/en-us/ef/core/providers/sqlite/?tabs=dotnet-core-cli>
   [Db browser sqlite]: <https://sqlitebrowser.org/>
   [DataAnnotations]: <https://docs.microsoft.com/en-us/dotnet/api/system.componentmodel.dataannotations?view=netcore-3.1>
