const expect = require('expect');
const contactManager = require('./contact-manager');

describe('contactManager', () => {
  let sut;

  beforeEach(() => {
    sut = contactManager();
  });

  describe('addTelephone', () => {
    it('a telephone number', () => {
      sut.addTelephone('0467123456');

      expect(sut.elementsCount()).toBe(10);
    });
    it('Numbers with a different base', () => {
      sut.addTelephone('0123456789');
      sut.addTelephone('1123456789');

      expect(sut.elementsCount()).toBe(20);
    });
    it('Number included in another', () => {
      sut.addTelephone('0123456789');
      sut.addTelephone('0123');

      expect(sut.elementsCount()).toBe(10);
    });
    it('Numbers with a common part', () => {
      sut.addTelephone('0412578440');
      sut.addTelephone('0412199803');
      sut.addTelephone('0468892011');
      sut.addTelephone('112');
      sut.addTelephone('15');

      expect(sut.elementsCount()).toBe(28);
    });
    it('Long list of numbers', () => {
      var numbers = require('./test-data/1000-telephones');

      for (var i = 0; i < numbers.length; i++) {
        var telephone = numbers[i];
        sut.addTelephone(telephone);
      }

      expect(sut.elementsCount()).toBe(45512);
    });
  });
});