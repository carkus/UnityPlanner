using System.Collections;

using System;
using System.Collections;
using System.Collections.Generic;

using JsonToDataContract;

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
        public void GroundActions(JsonNode _operators, JsonNode _objects)
        {

            List<HSPAction> action = new List<HSPAction>();
            
            foreach(var op in _operators.Members) {
                
                


                //string[] variables = [ param.name for param in operator.params ]            
                //objects = [ p_objs[param.type] for param in operator.params ]


            
            }

        }

        /*

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