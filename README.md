# BOT19COVIDTwitter

Bot para Twitter, que publica las actualizaciones connstantes sobre los casos recogidos del COVID-19. 

Se recogen a través de un documento .Json  (https://corona.lmao.ninja/countries). El cuál a su vez hace web scraping, recogiendo los datos de la página https://www.worldometers.info/coronavirus/. Estos datos concuerdan con los del gobierno de sanidad, pero éstos publican actualizaciones cada 12 horas. Con el bot se pretende tener actualizaciones constantes en dicha RRSS.

![Twitter account](https://github.com/SharkiCS/BOT19COVIDTwitter/blob/master/TwitterBOT4.png)

Aquí se puede ver la aplicación de Console (.Net framework) trabajando. Y el respectivo .JSON. 

![JSON](https://github.com/SharkiCS/BOT19COVIDTwitter/blob/master/TwitterBOT3.png)
![Program](https://github.com/SharkiCS/BOT19COVIDTwitter/blob/master/TwitterBOT.png)

En cada actualización obtenida, se informa en la consola. Se usan dos timers, uno para las actualizaciones de los promedios de muerte por cada mil personas y sus respectivas tasas. (Cada 12 horas hace una actualización). Y el otro timer busca una actualización en el .Json cada 5 minutos, una vez encontrada, publica el tweet al instante.

