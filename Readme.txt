Jonathan Melnik - 2021
Pixowl

Instrucciones:

Ejecutar el juego desde MainMenu.scene

El objetivo del juego es destruir a todos los asteroides y UFOs antes de perder las 3 vidas. 
Si se pierden las 3 vidas es game over.
Los UFOs van a aparecer cada cierto tiempo indefinidamente, y los asteroides son 3 fijos al principio.
Cada tanto apareceran power ups. Hay 3 tipos distintos: Shield, Bomb y Homing Missile.
El shield te hace invencible por cierto tiempo.
La bomba destruye todo dentro del radio de la explosion.
El misil homing establece un target y lo persigue.

Cheatcodes: 
	H: Misil homing
	B: bomba

	
Detalles de la implementacion:

	Primero que nada quiero agradecerles por la oportunidad.

	Segundo quiero recordarles que no tenia ninguna experiencia con DOTS y espero que lo tengan en cuenta a la hora de revisar el proyecto. 
	De todas maneras no creo que haya cometido algun error grave, pero tal vez hay aspectos en los que se podria haber sacado mas provecho de ECS o cosas que hice que no estan acordes a las buenas practicas de ECS.

	Trate de implementar todo lo que podia con DOTS.

	Donde saque mas provecho es en la deteccion de colisiones y en el codigo que se encarga del movimiento de las entidades.
Esos sistemas son los que manejan mas cantidad de entidades y los datos son compartidos entre muchos tipos de entidades distintas:
-ConstantMovementData es compartido por los disparos y los asteroides.
-MovementData es compartido por la nave y los UFOs

	Luego hay sistemas que se ejecutan sobre muchas entidades de distinto tipo:
-TeleportScreenBoundsSystem se ejecuta sobre todas las entidades y hace que estas se teleporten de un borde de la pantalla al otro
-PlayerEnemyCollisionSystem calcula colisiones entre todas las entidades de tipo player(la nave, los disparos y los misiles) y 
todas las entidades de tipo enemy(asteroides, ufos, disparos de ufos)

	Con el sistema de colision tuve muchas vueltas y varios refactors. Primero habia implementado la colision entre cada par de entidades y su resolucion en job systems distintos. Usaba EntityCommandBuffer para destruir las entidades. Luego empece a necesitar saber desde afuera de ECS cuando eran destruidos los asteroides y UFOs y entonces saque la resolucion de la colision y cree un manager, CollisioManager(MonoBehavior), que se encargue de eso. Al principio esto no me convencia porque queria que la logica permanezca en los job systems, pero despues me di cuenta que la mayoria de la logica igual la estaba haciendo en main thread, porque para destruir las entidades, el entity command buffer se ejecuta en main thread y ademas en la resolucion de la colision terminaba llamando a MonoBehaviors en la mayoria de los casos. De modo que sacar la resolucion fuera de ECS no supuso una baja en performance, eso sumado a que la resoluciones de colision suceden muy de vez en cuando, lo unico que sucede en cada frame y que tiene sentido optimizar es el calculo de la colision y eso quedo en DOTS.
	El otro refactoring importante que hice al sistema de colision consistio en generalizar un job system para que se encargue de calcular la colision entre todo par de entidades que sean Player vs Enemy. Anteriormente tenia un job system para cada par de entidades distinto(nave vs asteroides, nave vs ufos, disparo de nave vs asteroides, etc), pero eso resulto inviable a medida que fui agregando nuevas entidades porque cada nueva entidad podia crear varios pares de colision nuevos y tener un job system para cada uno era demasiado. Asi que resolvi este problema creando nuevos tags PlayerTag y EnemyTag y todas las entidades pertenecientes al player pasaron a tener PlayerTag ademas de su tag unico, lo mismo para todas las entidades enemigas pero con EnemyTag. De esa manera el sistema de colision se encargo entonces de calcular la colision entre entidades de tipo player y enemy y CollisionManager de resolver las colisiones chequeando los tags unicos de las entidades.

	Tuve un par de problemas. Hice un powerup 'bomba' que al tocar el powerup se detona y crea una onda expansiva. Para eso, lo que necesitaba era agrandar el collider de la bomba en un intervalo de tiempo. Esto fuera de DOTS es muy simple, pero DOTS no tiene una forma facil de hacer esto. En BombSystem se puede ver como lo resolvi. Accedo al puntero del collilder y modifico el radio de la esfera(bloque de codigo unsafe). Y aunque esto funciono, el problema es que el collider modificado terminaba siendo modificado para el prefab de la entidad y no para cada entidad bomba independiente. Entonces luego tuve que resetear el radio del collider cada vez que se usa la bomba. Y aunque esto no esta bien, de todas maneras lo deje asi porque nunca ocurre que el jugador use dos bombas una tras otra, entonces no se llega a percibir el problema de que el collider se haya reseteado para la primer bomba. En un juego real, directamente si quisiera implementar esta funcionalidad no usaria DOTS. Pero para este test quise dejarlo en DOTS.
	Otro problema que tuve fue que al lanzar muchos misiles homing obtenia un error que faltaba el componente Translation en la entidad HomingMissile. Esto no se si tiene que ver con como resuelvo la colision(fuera de DOTS) o si es un bug de DOTS. Mi suposicion de que es un bug viene porque en ningun momento creo una entidad sin Translation ni tampoco remuevo Translation de las entidades.

	Para concluir, DOTS me parece una herramienta muy interesante y me gusta poder mejorar la performance en mobile y asi obtener mejor framerate o mejorar el uso de bateria o evitar que el dispositivo recaliente. 
	Me gusto el nuevo paradigma y me gustaria que la tecnologia salga de estado preview y pase a ser parte del core de unity, o incluso que todo este hecho en DOTS. Las ventaja es varios ordenes de magnitud de rendimiento.
	