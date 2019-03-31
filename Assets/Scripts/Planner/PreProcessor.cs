using UnityEngine;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;
using Config;

namespace Planner
{

    public class PreProcessor
    {

        //Singleton
        private static PreProcessor instance;
        private PreProcessor()
        {
        }

        public static PreProcessor Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new PreProcessor();
                }
                return instance;
            }
        }

        //public void GroundActions (HSPOperator[] _operators, OBase[] _objects) {
        public List<HSPAction> GroundActions(List<HSPOperator> _operations, List<HSPPredicate> _objects)
        {

            List<HSPAction> actions = new List<HSPAction>();

            actions = BeginDeriveActions(_operations, _objects);

            return actions;
        }

        public List<HSPAction> BeginDeriveActions(List<HSPOperator> _operations, List<HSPPredicate> _objects) {

            List<HSPAction> actions = new List<HSPAction>();

            foreach (HSPOperator _operator in _operations) {

                List<string> variables = new List<string>();
                variables = DeriveVariables(_operator);

                List<HashSet<string>> objects = new List<HashSet<string>>();
                objects = DeriveObjects(_operator, _objects);

                List<List<string>> product = new List<List<string>>();
                product = CartesianProduct(objects);

                foreach (var prod in product) {

                    Dictionary<string, string> subst = new Dictionary<string, string>();

                    //'Replaces' Zip...
                    for (var i=0; i<variables.Count; i++) {
                        subst.Add(variables[i], prod[i]);
                    }

                    bool valid = true;

                    foreach (HSPLiteral _precondition in _operator._preconditions)
                    {
                        
                        if (_precondition.isNegative()) {

                            string lhs = prod[0];
                            string rhs = prod[1];

                            if (lhs == rhs) {
                                valid = false;
                                break;
                            }

                        }

                    }

                    if (valid) {
                        HSPAction action = ConvertOperatorIntoAction(_operator, subst);
                        actions.Add(action);
                    }

                }
            }

            return actions;

        }

        private HSPAction ConvertOperatorIntoAction(HSPOperator _operator, Dictionary<string, string> _subst) {

            List<string> args = new List<string>();

            List<HSPPredicate> preconditions = new List<HSPPredicate>();
            List<string> posEffects = new List<string>();
            List<string> negEffects = new List<string>();

            foreach(HSPTerm param in _operator._params) {
                string arg = _subst[param._name];
                args.Add(arg);
            }

            foreach(HSPLiteral _precondition in _operator._preconditions) {
                if (_precondition.isPositive()) {
                    HSPPredicate ground = _precondition._predicate.Ground(_subst);
                    preconditions.Add(ground);
                }
            }

            foreach (HSPLiteral effect in _operator._effects) {

                HSPPredicate ground = effect._predicate.Ground(_subst);

                if (effect.isPositive()) {
                    posEffects.Add(ground.GetString());
                }
                else if (effect.isNegative()) {
                    negEffects.Add(ground.GetString());
                }

            }

            return new HSPAction(_operator._name, args, preconditions, posEffects, negEffects);
        }


        private List<string> DeriveVariables(HSPOperator _op) {

            List<string> list = new List<string>();

            for (var j=0; j<_op._params.Count; j++) {
                list.Add(_op._params[j]._name);
            }

            return list;
        }

        private List<HashSet<string>> DeriveObjects(HSPOperator _op, List<HSPPredicate> _objects) {

            List<HashSet<string>> list = new List<HashSet<string>>();

            for (var i=0; i<_op._params.Count; i++) {
                string type = _op._params[i]._type;
                for (var j=0; j<_objects.Count; j++) {
                    if (_objects[j]._name == type) {
                        HashSet<string> group = new HashSet<string>();
                        for (var k=0; k<_objects[j]._args.Count; k++) {
                            group.Add(_objects[j]._args[k]._name);
                        }
                        list.Add(group);
                    }
                }
            }

            return list;
        }

        public static List<List<string>> CartesianProduct(List<HashSet<string>> objects) {

            List<List<string>> result = new List<List<string>>();

            foreach(var o1 in objects[0])
            {
                foreach(var o2 in objects[1]) 
                {
                    if (objects.Count == 3) {
                        foreach(var o3 in objects[2]) {
                            result.Add(new List<string>(new string[] { o1, o2, o3 }));
                        }
                    }
                    else {
                        result.Add(new List<string>(new string[] { o1, o2 }));
                    }
                }
            }

            return result;
        }

    }

}