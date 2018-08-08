using System;

namespace Askowl {
  public class Csv<T> {
    private T[] array;

    public static Csv<T> Instance(T[] seed) { return new Csv<T>().Seed(seed); }

    public Csv<T> Seed(T[] seedCsv) {
      array = seedCsv;
      return this;
    }

    public override string ToString() { return string.Join(",", Array.ConvertAll(array, x => x.ToString())); }
  }
}