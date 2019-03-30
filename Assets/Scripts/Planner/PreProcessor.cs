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

            for (var a=0; a<_operations.Count; a++) {

                List<string> variables = new List<string>();
                variables = DeriveVariables(_operations[a]);

                List<HashSet<string>> objects = new List<HashSet<string>>();
                objects = DeriveObjects(_operations[a], _objects);

                //TODO there's got to be a better way, for variable lengths.
                //But need Tuple to allow for identical values???
                List<Tuple<string, string, string>> product = new List<Tuple<string, string, string>>();
                if (objects.Count == 2) {
                    product = Product_3(objects[0], objects[1], null);
                    product.Remove(null);
                }

                if (objects.Count == 3) {
                    product = Product_3(objects[0], objects[1], objects[2]);
                }

                foreach (var prod in product) {

                    Dictionary<string, string> subst = new Dictionary<string, string>();

                    //Replaces Zip...
                    for (var i=0; i<variables.Count; i++) {
                        subst.Add(variables[i], prod[i]);
                        //subst = dict(zip(variables, prod))
                    }

                    bool valid = true;

                    for (var j=0; j<_operations[a]._preconditions.Count; j++) {

                        HSPLiteral pre = _operations[a]._preconditions[j];


                        if (pre.isNegative()) {

                            HSPPredicate predicate = pre._predicate;

                            //string lhs = subst[predicate._args[0]._name];
                            //string rhs = subst[predicate._args[1]._name];

                            Debug.Log(subst[predicate._args[0]._name] + " : " + pre.isNegative());

                        }

                        if (valid) {

                            //HSPAction action = ConvertOperatorIntoAction(_operations[a], subst);

                        }



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

        private HSPAction ConvertOperatorIntoAction(HSPOperator _operator, Dictionary<string, List<string>> _subst) {

            List<string> args = new List<string>();

            HashSet<string> posEffect = new HashSet<string>();
            HashSet<string> negEffect = new HashSet<string>();

            foreach (var effect in _operator._effects) {



            }


            return new HSPAction();
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

            foreach(var t1 in a)
            {
                foreach(var t2 in b) {
                    result.Add(Tuple.Create<string, string>(t1, t2));
                }
            }

            return result;
        }*/

        public static List<Tuple<string, string, string>> Product_3(HashSet<string> a, HashSet<string> b, HashSet<string> c) {

            List<Tuple<string, string, string>> result = new List<Tuple<string, string, string>>();

            foreach(var t1 in a)
            {
                foreach(var t2 in b) 
                {
                    if (c != null) {
                        foreach(var t3 in c)
                            result.Add(Tuple.Create<string, string, string>(t1, t2, t3));
                    }
                    else {
                        result.Add(Tuple.Create<string, string, string>(t1, t2, null));
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