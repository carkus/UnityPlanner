
using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

public class HSPLiteral {

    private HSPPredicate _predicate;
    private bool _positive;
   
    public HSPLiteral(HSPPredicate predicate, bool positive) {
        _predicate = predicate;
        _positive = positive;
    }

    public HSPPredicate getPredicate() {
        return _predicate;
    }

    public bool isPositive() {
        return _positive;
    }

    public bool isNegative() {
        return !_positive;
    }

    /*public HSPLiteral ground(HSPPredicate subst) {
        HSPPredicate ground_predicate = predicate.ground(subst);
        if (isPositive()) {
            return HSPLiteral.positive(ground_predicate);
        }
        else if (isNegative()) {
            return HSPLiteral.negative(ground_predicate);
        }
    } */

    /* public HSPLiteral positive(HSPPredicate predicate) {
        return new HSPLiteral(predicate, true); 
    }

    public HSPLiteral negative(HSPPredicate predicate) {
        return new HSPLiteral(predicate, false); 
    }*/


    /*
    @property
    def predicate(self):
        return self._predicate

    def is_positive(self):
        return self._positive

    def is_negative(self):
        return not self._positive

    def ground(self, subst):
        ground_predicate = self._predicate.ground(subst)
        if self.is_positive():
            return Literal.positive(ground_predicate)
        if self.is_negative():
            return Literal.negative(ground_predicate)

    @classmethod
    def positive(cls, predicate):
        return Literal(predicate, True)

    @classmethod
    def negative(cls, predicate):
        return Literal(predicate, False)    
    */


}