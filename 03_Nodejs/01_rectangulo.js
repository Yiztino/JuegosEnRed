class Rectangulo {
  constructor(alto, ancho) {
    this._alto = alto;
    this._ancho = ancho;
  }
  // Getter
  get area() {
    
    return this.calcArea();
  }
  get perimetro() {
    return this.calcPerimetro();
  }
  // Método
  calcArea() {
    return this.alto * this.ancho;
  }

  calcPerimetro(){
    return (this.alto*2)+(this.ancho*2)
  }

  *obtenerLados(){
    yield this.alto; 
    yield this.ancho;
    yield this.alto; 
    yield this.ancho;
  }
}

let cuadrado = new Rectangulo(10,10);



console.log("Area: "+cuadrado.area);
console.log("Perimetro: "+cuadrado.perimetro);