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

                //TODO there's got to be a better way, for variable lengths.
                //But need Tuple to allow for identical values???
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

        /*''' Given a mapping from variable to constants `subst`return a ground action. '''
        args = [ str(subst.get(param.name, param)) for param in operator._params ]
        precond = { str(l.predicate.ground(subst)) for l in operator._precond if l.is_positive() }
        pos_effect = set()
        neg_effect = set()
        for eff in operator._effects:
            ground_predicate = eff.predicate.ground(subst)
            if eff.is_positive():
                pos_effect.add(str(ground_predicate))
            elif eff.is_negative():
                neg_effect.add(str(ground_predicate))
        return Action(operator._name, args, precond, pos_effect, neg_effect)*/

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

        /*public static IEnumerable<T[]> Zip<T>(params IEnumerable<T>[] iterables) {

            IEnumerator<T>[] enumerators = Array.ConvertAll(iterables, (iterable) => iterable.GetEnumerator());

            while (true) {
                int index = 0;
                T[] values = new T[enumerators.Length];

                foreach (IEnumerator<T> enumerator in enumerators) {

                    if (!enumerator.MoveNext())
                        yield break;
                        values[index++] = enumerator.Current;

                }

                yield return values;
            }
        }

         public static List<Tuple<string, string>> Product_2(HashSet<string> a, HashSet<string> b) {

            List<Tuple<string, string>> result = new List<Tuple<string, string>>();

            foreach(var o1 in a)
            {
                foreach(var o2 in b) {
                    result.Add(Tuple.Create<string, string>(o1, o2));
                }
            }

            return result;
        }*/

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


        //}

        //IEnumerable<string[]> zippedValues = Zip(e1, e2);


        
        /* IEnumerable<int> e1 = new List<int>() { 1, 2, 3};
        IEnumerable<int> e2 = new List<int>() { 4, 5, 6};
        IEnumerable<int[]> zippedValues = Zip(e1, e2);

        foreach(int[] arr in zippedValues)  {

            Debug.Log("{");

            foreach(int val in arr) {
                Debug.Log(val);
                Debug.Log("}");
            }

        }*/



        


        private List<HSPLiteral> DerivePreConditions(HSPOperator _op) {

            List<HSPLiteral> list = new List<HSPLiteral>();
            
            //for (var i=0; i<_op.Count; i++) {

                //list.Add(_ops[i]._name);
            //}

            return list;
        }

       
        /*


        putdown(box2, left, room1)
        pickup(box3, right, room1)
        pickup(box2, left, room1)
        move(room1, room3)
        putdown(box3, right, room3)
        move(room3, room2)
        putdown(box2, left, room2)


        def ground_actions(self, d_ops, p_objs):
        #Return all actions grounded from all operators in `domain`
        #with respect to objects defined in the problem.
        actions = []
        for operator in d_ops:
            
            
            variables = [ param.name for param in operator.params ]            
            objects = [ p_objs[param.type] for param in operator.params ]


            for prod in itertools.product(*objects):

                subst = dict(zip(variables, prod))

                valid = True

                for pre in operator.precond:                    
                    if pre.is_negative():
                        predicate = pre.predicate
                        assert(predicate.name == '=')
                        lhs = subst[predicate.args[0]]
                        rhs = subst[predicate.args[1]]
                        if lhs == rhs:
                            valid = False
                            break


                if valid:
                    action = self.convert_operator_to_action(operator, subst)
                    actions.append(action)
                    #print action.label


        return actions

        def convert_operator_to_action(self, operator, subst):
        ''' Given a mapping from variable to constants `subst`return a ground action. '''
        args = [ str(subst.get(param.name, param)) for param in operator._params ]
        precond = { str(l.predicate.ground(subst)) for l in operator._precond if l.is_positive() }
        pos_effect = set()
        neg_effect = set()
        for eff in operator._effects:
            ground_predicate = eff.predicate.ground(subst)
            if eff.is_positive():
                pos_effect.add(str(ground_predicate))
            elif eff.is_negative():
                neg_effect.add(str(ground_predicate))
        return Action(operator._name, args, precond, pos_effect, neg_effect)

         */


    }

}